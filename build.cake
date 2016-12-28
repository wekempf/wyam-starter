#tool nuget:?package=Wyam&prerelease
#addin nuget:?package=Cake.Wyam&prerelease

var recipe = "Blog";
var theme = "CleanBlog";
var isLocal = BuildSystem.IsLocalBuild;
var gitPagesRepo = "https://wekempf:" + EnvironmentVariable("GitHubPersonalAccessToken") + "@github.com/wekempf/wyam-starter.git";
var gitPagesBranch = "gh-pages";

var target = Argument("target", isLocal ? "Default" : "CIBuild");

Task("Build")
    .Does(() => {
        Wyam(CreateSettings(false));
    });
    
Task("Preview")
    .Does(() => {
        Wyam(CreateSettings(true));        
    });
    
Task("Default")
    .IsDependentOn(isLocal ? "Preview" : "CIBuild");
    
Task("CIBuild")
    .IsDependentOn("Build")
    .Does(() => {
        if (!isLocal) {
            if (GitClonePages() != 0) {
                throw new Exception("Unable to clone Pages.");
            }
            var dirs = GetDirectories("./pages/*")
                .Except(GetDirectories("./pages/.git"), DirectoryPathComparer.Default);
            DeleteDirectories(dirs, true);
            var files = GetFiles("./pages/*");
            DeleteFiles(files);
            CopyFiles("./output/**/*", "./pages", true);
            if (GitCommitPages() != 0) {
                throw new Exception("Unable to commit Pages.");
            }
            if (GitPushPages() != 0) {
                throw new Exception("Unable to publish Pages.");
            }
        }
    });

RunTarget(target);

public class DirectoryPathComparer : IEqualityComparer<DirectoryPath>
{
    public bool Equals(DirectoryPath x, DirectoryPath y)
    {
        return string.Equals(x.FullPath, y.FullPath, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(DirectoryPath x)
    {
        return x.FullPath.GetHashCode();
    }

    private static DirectoryPathComparer instance = new DirectoryPathComparer();
    public static DirectoryPathComparer Default
    {
        get { return instance; }
    }
}

WyamSettings CreateSettings(bool preview)
{
    return new WyamSettings {
        Recipe = recipe,
        Theme = theme,
        Preview = preview,
        Watch = preview
    };
}

int GitClonePages() {
    return StartProcess("git", new ProcessSettings {
        Arguments = "clone " + gitPagesRepo + " -b " + gitPagesBranch + " pages" 
    });
}

int GitCommitPages() {
    var result = StartProcess("git", new ProcessSettings {
        Arguments = "add .",
        WorkingDirectory = "./pages"
    });
    if (result != 0) {
        return result;
    }
    result = StartProcess("git", new ProcessSettings {
        Arguments = "commit -m \"Publishing pages\"",
        WorkingDirectory = "./pages"
    });
    return result;
}

int GitPushPages() {
    return StartProcess("git", new ProcessSettings {
        Arguments = "push",
        WorkingDirectory = "./pages"
    });
}
