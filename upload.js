var multer  = require('multer'),
    path    = require('path'),
    fs      = require('fs-extra'),
    _       = require('lodash');

var securityRE = /[^A-Za-z0-9.\/_-]/ig;
var securityRE2= /\.\./ig;
var slashRE = /\//ig;

var exportPath = 'assetbundles';

module.exports = multer.diskStorage({

  destination: function (req, file, cb) {

    req.asset = _.extend({
      build: 'development',
      hash: 0,
      platform: 'OSX',
    }, _.pick(req.body, ['build', 'hash', 'platform', 'assetbundle']));

    // Get filename and apply a security normalization over the filename
    req.filename = req.asset.assetbundle || file.originalname;
    req.filename = path.normalize(req.filename.replace(securityRE, '').replace(securityRE2, '')).toLowerCase();

    var dir;

    if (req.asset.hash)
      dir = path.join('.', exportPath, req.asset.platform, 'all');
    else
      dir = path.join('.', exportPath, req.asset.platform, req.asset.build, path.dirname(req.filename));

    fs.ensureDir(dir, () =>
      cb(null, dir)
    );
  },

  filename: function (req, file, cb) {

    var filename = req.filename;

    if (req.asset.hash) {

      // Cache file flatterns the path and appends the hash
      var cachename = filename.replace(slashRE, "-") + '_' + req.asset.hash;

      var cachepath = path.join('.', exportPath, req.asset.platform, 'all', cachename);
      var linkdst   = path.join('.', exportPath, req.asset.platform, req.asset.build, filename);
      var linksrc   = path.relative(path.dirname(linkdst), cachepath);

      var asset = req.asset || {};

      _.extend(asset, {
        file      : filename,
        linkdst   : linkdst,
        linksrc   : linksrc,
        cachepath : cachepath,
        cachename : cachename,
      });

      fs.ensureDir(path.dirname(linkdst), () =>
        fs.symlink(linksrc, linkdst, 'file', (err) => {
          if (err)
            fs.remove(linksrc, () =>
              fs.symlink(linksrc, linkdst, 'file', () => {})
            )
          }
        )
      );

      cb(null, cachename);

    } else {

      cb(null, path.basename(filename));

    }
  }
});
