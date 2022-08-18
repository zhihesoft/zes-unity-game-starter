const { src, dest } = require('gulp');
const browserify = require("browserify");
const source = require('vinyl-source-stream');
const sourcemaps = require('gulp-sourcemaps');
const tsify = require("tsify");
const uglify = require("gulp-uglify");
const buffer = require('vinyl-buffer');

function build() {

    return browserify({
        basedir: '.',
        standalone: "livexports",
        debug: true,
        ignoreMissing: true,
        entries: ['./src/index.ts'],
        cache: {},
        packageCache: {}
    })
        .plugin(tsify)
        .bundle()
        .pipe(source('main.bytes'))
        .pipe(buffer())
        .pipe(sourcemaps.init({ loadMaps: true }))
        .pipe(uglify({
            "keep_fnames": "true"
        }))
        .pipe(sourcemaps.write('./'))
        .pipe(dest("out"));

    // return src(['dist/*.js', 'node_modules/**/*.js'])
    //     .pipe(concat("main.js"))
    //     .pipe(dest("./out/"));
}

exports.build = build;