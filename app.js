var express = require('express'),
    multer  = require('multer'),
    upload  = require('./upload'),
    _       = require('lodash');

var app = express();

app.use('/assetbundles', express.static('assetbundles'));

app.post('/', multer({storage: upload}).any(), function(req, res) {
    console.log('asset', req.asset);
    res.status(204).end()
});

app.listen(3000);
