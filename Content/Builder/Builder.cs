/// <summary>
/// Entry point for your Content Builder project.
/// When run it builds your content according to the content collection.
/// </summary>
/// <remarks>
/// For more details regarding the Content Builder, see the MonoGame documentation:
///
///    https://docs.monogame.net/articles/getting_started/content_pipeline/content_builder_project.html
///
/// </remarks>

using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Framework.Content.Pipeline.Builder;


// If you need to debug the content build process you can enable
// this, build your game, then attach the debugger when prompted.
//System.Diagnostics.Debugger.Launch();


var builder = new Builder();

// We depend on the build arguments from the command line which
// are typically passed from your game project build commands.
if (args.Length > 0)
    builder.Run(args);

else
{
    // NOTE: You could create your own ContentBuilderParams
    // and configure it and pass it to Run if you don't want
    // to depend on the caller passing the right arguments.

    // Calling this will just return the help message.
    builder.Run(new ContentBuilderParams { Mode = ContentBuilderMode.None });
}

return builder.FailedToBuild > 0 ? -1 : 0;


public class Builder : ContentBuilder
{
    public override IContentCollection GetContentCollection()
    {
        var content = new ContentCollection();

        // No content is imported by default.
        // Define your content collection rules here.
        //
        //   https://docs.monogame.net/articles/getting_started/content_pipeline/content_builder_project.html?#creating-your-contentcollection
        //
        // Examples:
        //
        // Import all content in the Assets folder using the default importer for their file type.
        // content.Include<WildcardRule>("*");
        //
        // Only copy content from the assets folder rather than build it with the pipeline.
        // content.IncludeCopy<WildcardRule>("*.json");
        // 
        // Exclude assets that match the pattern., only required overriding a default import behavior.
        // content.Exclude<WildcardRule>("Font/*.txt");
        //
        // Include a specific asset with processor parameters.
        // content.Include("Models/character.glb", new FbxImporter(),
        //    new MeshAnimatedModelProcessor()
        //    {
        //        Scale = 100.0f
        //    }
        // );

        return content;
    }
}
