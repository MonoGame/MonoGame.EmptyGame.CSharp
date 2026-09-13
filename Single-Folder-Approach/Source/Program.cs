// MonoGame - Copyright (C) MonoGame Foundation, Inc
// This file is subject to the terms and conditions defined in
// file 'LICENSE.md', which is part of this source code package.

// This file is the entry point for every platform project except Android, which starts at MainActivity.cs instead.
// Everything in Source/ is compiled into every project, so platform-specific code goes behind the compiler symbols the .NET SDK defines per
// target framework: __IOS__, __ANDROID__, WINDOWS (the modern IOS and ANDROID spellings work too).
// Game1.cs uses __IOS__ the same way.

#if __IOS__
using Foundation;
using UIKit;

namespace EmptyGame.iOS;

[Register("AppDelegate")]
internal class Program : UIApplicationDelegate
{
    private static Game1 _game;

    internal static void RunGame()
    {
        _game = new Game1();
        _game.Run();
    }

    public override void FinishedLaunching(UIApplication app)
    {
        RunGame();
    }

    private static void Main(string[] args)
    {
        UIApplication.Main(args, null, typeof(Program));
    }
}
#elif !__ANDROID__
using EmptyGame;

using var game = new Game1();
game.Run();
#endif
