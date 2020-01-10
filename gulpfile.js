var gulp = require("gulp");
var fs = require("fs");
var args = require("yargs").argv;
var bump = require("gulp-bump");
var dotnet = require("gulp-dotnet-cli");
var path = require("path");

//optional, lets you do .pipe(debug()) to see whats going on
var debug = require("gulp-debug");

var project = JSON.parse(fs.readFileSync("./package.json"));

var config = {
  name: project.name,
  version: project.version,
  mode: args.mode || "Debug",
  output: ".build/deploy",
  deployTarget: args.deployTarget,
  releasenotesfile: "ReleaseNotes.md",
  buildNumber: args.build || "000",
  source: '<>',
  apiKey: '<>'
}

gulp.task("default", [ "restore", "compile", "test" ]);

gulp.task("restore", () => {
    return gulp.src(config.name + ".sln", { read: false })
        .pipe(dotnet.restore({
            configfile: "nuget.config",
            version: config.version
        }));
});

gulp.task("compile", ["restore"], function () {
  return gulp.src(config.name + ".sln", { read: false })
      .pipe(dotnet.clean({
          verbosity: 'quiet'
      }))
      .pipe(dotnet.build({
          configuration: config.mode,
          version: config.version,
          noIncremental: true
      }));
});

gulp.task("test", ["compile"], () => {
    return gulp.src("**/*.Tests*.csproj", { read: false })
        .pipe(dotnet.test({
            additionalArgs: "/p:CollectCoverage=true",
            noBuild: true,
            configuration: config.mode
        }));
});

gulp.task("package", ["test"], function() {
        return gulp.src(config.name + "/" + config.name + ".csproj", { read: false })
            .pipe(dotnet.pack({
                output: path.join(process.cwd(), config.output),
                version: config.version,
                configuration: config.mode,
                noBuild: true,
                noRestore: true
            }));
});

gulp.task('deploy', ["package"], function () {
    return gulp
        .src(config.output + '/*.' + config.version + '.nupkg')
        .pipe(dotnet.push({
            source: config.source,
            apiKey: config.apiKey
        }));
});

gulp.task("bump:patch", function() {
  return gulp
    .src("./package.json")
    .pipe(bump({ type: "patch"}))
    .pipe(gulp.dest("./"));
});

gulp.task("bump:minor", function() {
  return gulp
    .src("./package.json")
    .pipe(bump({ type: "minor"}))
    .pipe(gulp.dest("./"));
});

gulp.task("bump:major", function() {
  return gulp
    .src("./package.json")
    .pipe(bump({ type: "major"}))
    .pipe(gulp.dest("./"));
});
