var multer  = require('multer'),
    path    = require('path'),
    fs      = require('fs-extra'),
    _       = require('lodash');

var dir = 'assetbundles';
var all = path.join('.', dir, 'all');

module.exports = multer.diskStorage({

  destination: function (req, file, cb) {

    var dir = path.join(all, req.asset.platform);

    fs.ensureDir(dir, () =>
      cb(null, dir)
    );
  },

  filename: function (req, file, cb) {

    var filename  = file.originalname;
    var basename  = path.basename(filename);
    var extension = path.extname(filename).toLowerCase();
    var finalname = filename + req.asset.hash;

    var builddir  = path.join('.', dir, req.asset.build, req.asset.platform);
    var linkdst   = path.join(builddir, filename);
    var linksrc   = path.join('..', '..', 'all', req.asset.platform, finalname);

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

    fs.ensureDir(builddir, () =>
      fs.symlink(linksrc, linkdst, 'file', (err) => {
        if (err)
          fs.remove(linkdst, () =>
            fs.symlink(linksrc, linkdst, 'file', () => {})
          )
        }
      )
    );

    cb(null, finalname);
  }
});
