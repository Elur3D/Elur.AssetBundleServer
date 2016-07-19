var multer  = require('multer'),
    path    = require('path'),
    fs      = require('fs-extra'),
    _       = require('lodash');

var dir = 'assetbundles';
var all = path.join('.', dir, 'all');

fs.ensureDirSync(all);

module.exports = multer.diskStorage({

  destination: function (req, file, cb) {
    cb(null, all);
  },

  filename: function (req, file, cb) {

    var filename  = file.originalname;
    var basename  = path.basename(filename);
    var extension = path.extname(filename).toLowerCase();
    var finalname = filename + req.asset.hash;

    var builddir  = path.join('.', dir, req.asset.build);
    var linksrc   = path.join(builddir, filename);
    var linkdst   = path.join('..', 'all', finalname);

    var asset = req.asset || {};

    _.extend(asset, {
      filename  : filename,
      basename  : basename,
      extension : extension,
      finalname : finalname,
      builddir  : builddir,
      linksrc   : linksrc,
      linkdst   : linkdst,
    });

    fs.ensureDir(builddir, function(err) {
      fs.ensureLink(linksrc, "linkdst", function(err) {});
    });

    cb(null, finalname);
  }
});
