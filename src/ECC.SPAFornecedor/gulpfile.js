/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require("gulp-uglify");
var ngAnnotate = require("gulp-ng-annotate");
 
gulp.task('minify', function () {
    // place code for your default task here
	//return gulp.src([
	//	"Scripts/SPAFranquia/*.js",
	//	"Scripts/SPAFranquia/dataCotacao/*.js"])





	return gulp.src([
		//'js/**',
		//'html/**',
		'Scripts/**/!(*.html|*.map)'

		// gulp.src(['client/js/**/!(*.tests|*.fixtures|*.mocks)*.js'])
		//'!html/**/*.html',
		//'Scripts/**'
		// "Scripts/*.js"

	 


	], { base: '.' })
		.pipe(ngAnnotate())
		.pipe(uglify())
		.pipe(gulp.dest("dist"));
});