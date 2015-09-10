using ImpulsaGameJamsAtari.Forja.Trampas;
using ImpulsaGameJamsAtari.Personajes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;

namespace ImpulsaGameJamsAtari.Tutorial {
    public class Tutorial1 : BaseMenuScreen {

        ContentManager content;
        Texture2D fondo;

        Pinchos pinchos;
        Goblin goblinCaminando;
        Goblin goblinAtacando;
        Goblin goblinP1;
        Goblin goblinP2;

        float selectDelay;
        float selectMaxDelay;
        bool select;

        public Tutorial1()
            : base("Cómo Jugar") {

                YTitlePosition = 50;
                YStartingEntryPosition = 550;

                MenuEntry leyenda = new MenuEntry(this, "Leyenda");
                leyenda.Selected += leyenda_Selected;
                MenuEntries.Add(leyenda);

                selectDelay = 0f;
                selectMaxDelay = 1000f;
                select = false;
        }

        void leyenda_Selected(object sender, PlayerIndexEventArgs e) {
            ScreenManagerController.AddScreen(new Tutorial2(), null);
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            fondo = content.Load<Texture2D>("Texturas/tutorial-1");

            pinchos = new Pinchos(ScreenManagerController);
            pinchos.Escala = new Vector2(5, 5);
            pinchos.Posicion = new Vector2(460, 290);
            pinchos.AlturaInicial = (int)pinchos.Posicion.Y;
            pinchos.LoadTutorial();

            goblinCaminando = new Goblin(ScreenManagerController);
            goblinCaminando.LoadTutorial();
            goblinCaminando.Posicion = new Vector2(280, 141);

            goblinAtacando = new Goblin(ScreenManagerController);
            goblinAtacando.LoadTutorial();
            goblinAtacando.Posicion = new Vector2(480, 141);

            goblinP1 = new Goblin(ScreenManagerController);
            goblinP1.LoadTutorial();
            goblinP1.Posicion = new Vector2(280, 255);

            goblinP2 = new Goblin(ScreenManagerController);
            goblinP2.LoadTutorial();
            goblinP2.Posicion = new Vector2(320, 255);
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();
            pinchos.UnloadContent();
            goblinCaminando.UnloadContent();
            goblinAtacando.UnloadContent();
            goblinP1.UnloadContent();
            goblinP2.UnloadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);            

            pinchos.UpdateTutorial(gameTime);
            goblinCaminando.UpdateCaminandoTutorial(gameTime);
            goblinAtacando.UpdateAtacandoTutorial(gameTime);

            goblinP1.Update(gameTime);
            goblinP2.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            SpriteFont font = ScreenManagerController.Font;
            selectDelay += gameTime.ElapsedGameTime.Milliseconds;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            spriteBatch.Draw(
                fondo,
                new Vector2(0,0),
                Color.White * TransitionAlpha
            );

            pinchos.DrawTutorial(gameTime, TransitionAlpha);
            goblinCaminando.DrawTutorial(gameTime, TransitionAlpha);
            goblinAtacando.DrawTutorial(gameTime, TransitionAlpha);

            if (selectDelay >= selectMaxDelay) {
                select = !select;
                selectDelay = 0f;
            }

            goblinP1.DrawTutorial(gameTime, TransitionAlpha, !select);
            goblinP2.DrawTutorial(gameTime, TransitionAlpha, select);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
