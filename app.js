var express = require('express'),
    multer  = require('multer'),
    upload  = require('./upload'),
    _       = require('lodash');

var app = express();

app.get('/', function(req, res){
  res.send('hello world');
});

app.use('/', function (req, res, next) {
  req.asset = _.extend({
    build: 'development',
    hash: 0,
    platform: 'OSX',
  }, _.pick(req.body, ['build', 'hash']));
  next();
});

app.post('/', multer({storage: upload}).any(), function(req, res) {
    console.log('req', req.body) // form fields
    console.log('files', req.files) // form files
    console.log('asset', req.asset);
    res.status(204).end()
});

app.listen(3000);
