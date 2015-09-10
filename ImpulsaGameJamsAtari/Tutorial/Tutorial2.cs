using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;

namespace ImpulsaGameJamsAtari.Tutorial {
    public class Tutorial2 : BaseMenuScreen {
        ContentManager content;
        Texture2D fondo;

        public Tutorial2()
            : base("Leyenda") {
                YTitlePosition = 50;
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            fondo = content.Load<Texture2D>("Texturas/tutorial-2");
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            SpriteFont font = ScreenManagerController.Font;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            spriteBatch.Draw(
                fondo,
                new Vector2(0,0),
                Color.White * TransitionAlpha
            );

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
