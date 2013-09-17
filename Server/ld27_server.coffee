#
# The Legend of Epikouros
#   ~ a game made in 48h by Erhune for Ludum Dare 27
#
# Copyright (c) 2013 Erhune <erhune@gmail.com>
#
# Feel free to contact me any time!
#


environment = process.env.RAILS_ENV || 'development'
console.log "LD27 server (#{environment})"

pgUser = process.env.PG_USER || 'postgres'
pgPassword = process.env.PG_PASSWORD || 'pass'
pgDatabase = 'ld27'

pg = require('pg')
conString = "postgres://#{pgUser}:#{pgPassword}@localhost/#{pgDatabase}";
console.log "Using PostgreSQL database #{pgDatabase} on localhost with user #{pgUser}"

runs = []
runIps = []
authorizedToRun = null

pg.connect conString, (err, client, done) ->
  if err?
    console.error "Error fetching DB connection from pool:", err
    process.exit(1)

  client.query 'SELECT * FROM runs ORDER BY cycle', null, (err, result) ->
    done() # return connection to the pool

    if err?
      console.error "Error running initial load query", err
      process.exit(1)

    console.log "Reloading #{result.rows.length} runs..."
    for row in result.rows
      run =
        cycle: row.cycle
        nickname: row.nickname
        title: row.title
        actions: row.actions
      runIp = row.ip
      console.log "Reloading run", run, "for IP", runIp
      runs.push(run)
      runIps.push(runIp)

process.on 'uncaughtException', (err) ->
  console.error "Uncaught exception:", err
  console.trace()

adminIp = process.env.ADMIN_IP || '127.0.0.1'
crossDomain = """<?xml version="1.0"?>
                 <cross-domain-policy>
                 <allow-access-from domain="*" />
                 </cross-domain-policy>"""

previousRun = (ip) ->
  return null if ip == adminIp
  for runIp, i in runIps
    return runs[i] if runIp == ip
  null

http = require('http')
querystring = require('querystring')
server = http.createServer (request, response) ->

  ip = request.headers['x-forwarded-for'] or request.connection.remoteAddress

#  console.log request.method, request.url, request.headers
#  console.log request.connection.remoteAddress

  if request.method == 'GET' and request.url == '/crossdomain.xml'
    response.writeHead(200, {'Content-Type': 'text/plain'})
    response.end(crossDomain)
  else if request.method == 'GET' and request.url == '/api/load'
    console.log "Load request from", ip
    previous = previousRun(ip)
    console.log "Previous run from this ip is", previous
    response.writeHead(200, {'Content-Type': 'text/plain'})
    responseData =
      runs: runs
      alreadyPlayedAs: previous?.nickname
    response.end(JSON.stringify(responseData))
  else if request.method == 'GET' and request.url[0...9] == '/api/poll'
    console.log "Poll request from", ip
    data = querystring.parse(request.url[10..])
    fromCycle = data.from
    console.log "Poll from cycle", fromCycle
    response.writeHead(200, {'Content-Type': 'text/plain'})
    responseData =
      runs: run for run in runs when run.cycle > fromCycle
    console.log "Found", responseData.runs.length, "new runs for this player"
    response.end(JSON.stringify(responseData))
  else if request.method == 'POST' and request.url == '/api/start'
    console.log "Start request from", ip
    request.setEncoding('utf8')
    request.on 'readable', ->
      request.body = request.read()
    request.on 'end', ->
      previous = previousRun(ip)

      if previous != null
        response.writeHead(200, {'Content-Type': 'text/plain'})
        response.end('Already played')
        return

      if authorizedToRun == null or authorizedToRun.at < Date.now() + 15000 or authorizedToRun.ip == ip
        data = querystring.parse(request.body)
        authorizedToRun =
          ip: ip
          nickname: data.nickname
          at: Date.now()
        console.log "Authorized to run:", authorizedToRun
        response.writeHead(200, {'Content-Type': 'text/plain'})
        response.end('OK')
        return

      console.log "Not authorized to run, there is already this in progress:", authorizedToRun
      response.writeHead(200, {'Content-Type': 'text/plain'})
      response.end('KO')
  else if request.method == 'POST' and request.url == '/api/run'
    console.log "Run result from", ip
    request.setEncoding('utf8')
    request.body = ''
    request.on 'readable', ->
      request.body += request.read()
    request.on 'end', ->
      if ip == authorizedToRun?.ip
        console.log "Raw run data is", request.body
        run = JSON.parse(request.body)
        console.log "Received run from authorized #{ip}:", run
        run.cycle = runs.length + 1
        console.log "Assigning cycle #{run.cycle} to this run"
        runs.push(run)
        runIps.push(ip)
        authorizedToRun = null
        response.writeHead(200, {'Content-Type': 'text/plain'})
        response.end("#{run.cycle}")

        # Save to DB
        pg.connect conString, (err, client, done) ->
          if err?
            console.error "Error fetching DB connection from pool:", err
            return

          client.query 'INSERT INTO runs(cycle, nickname, title, ip, actions, created_at) VALUES ($1::int4, $2::varchar, NULL, $3::inet, $4::json, NOW())', [run.cycle, run.nickname, ip, JSON.stringify(run.actions)], (err, result) ->
            done() # return connection to the pool

            if err?
              console.error "Error running insert run query", err
            else
              console.log "Run #{run.cycle} saved in the DB"
      else
        response.writeHead(200, {'Content-Type': 'text/plain'})
        response.end("0")
  else
    console.error "Unknown request:", request.method, request.url

server.listen(8080)


setInterval ->
  now = new Date()
  console.log "STATUS", now, "[running for #{process.uptime() | 0} seconds]", process.memoryUsage()
, 10000