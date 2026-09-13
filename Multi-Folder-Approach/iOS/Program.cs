using Foundation;
using UIKit;

namespace EmptyGame.iOS
{
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

        static void Main(string[] args)
        {
            UIApplication.Main(args, null, typeof(Program));
        }
    }
}
