using ImpulsaGameJamsAtari.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;
using ScreenManager.PantallasBases;
using System;
using System.Collections.Generic;

namespace ImpulsaGameJamsAtari.Menus
{
    class MenuPrincipal : BaseMenuScreen
    {
        Texture2D goblinT;
        List<int> frameActualG = new List<int>();
        List<float> tiempoTranscurridoG = new List<float>();

        Texture2D humanoT;
        List<int> frameActualH = new List<int>();
        List<float> tiempoTranscurridoH = new List<float>();

        ContentManager content;

        int startingPositionG;
        int spacingPositionG;
        int startingPositionH;
        int spacingPositionH;

        public MenuPrincipal(string titulo) : base(titulo)
        {
            YStartingEntryPosition = 260;
            YOffSetPosition = 5;

            MenuEntry nivel1 = new MenuEntry(this, "Nivel 1");
            MenuEntry nivel2 = new MenuEntry(this, "Nivel 2");
            MenuEntry nivel3 = new MenuEntry(this, "Nivel 3");
            MenuEntry nivel4 = new MenuEntry(this, "Nivel 4");
            MenuEntry nivel5 = new MenuEntry(this, "Nivel 5");

            MenuEntry infinito = new MenuEntry(this, "Infinito");

            MenuEntry tutorial = new MenuEntry(this, "Cómo Jugar");

            MenuEntry creditos = new MenuEntry(this, "Créditos");

            nivel1.Selected += NivelSelected;
            nivel2.Selected += NivelSelected;
            nivel3.Selected += NivelSelected;
            nivel4.Selected += NivelSelected;
            nivel5.Selected += NivelSelected;

            infinito.Selected += NivelSelected;
            tutorial.Selected += tutorial_Selected;
            creditos.Selected += creditos_Selected;

            MenuEntries.Add(nivel1);
            MenuEntries.Add(nivel2);
            MenuEntries.Add(nivel3);
            MenuEntries.Add(nivel4);
            MenuEntries.Add(nivel5);
            MenuEntries.Add(infinito);
            MenuEntries.Add(tutorial);
            MenuEntries.Add(creditos);
        }

        private void tutorial_Selected(object sender, PlayerIndexEventArgs e) {
            ScreenManagerController.AddScreen(new Tutorial.Tutorial1(), null);
        }

        public override void LoadContent() {
            base.LoadContent();

            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            Random ran = new Random();
            for (int i = 0; i < 5; i++) {
                frameActualG.Add(ran.Next(0,5));
                tiempoTranscurridoG.Add(0);
                startingPositionG = ran.Next(30,201);
                spacingPositionG = ran.Next(30, 60);

                frameActualH.Add(ran.Next(0, 5));
                tiempoTranscurridoH.Add(0);
                startingPositionH = ran.Next(570, 741);
                spacingPositionH = ran.Next(30, 60);
            }

            goblinT = content.Load<Texture2D>("Texturas/goblin-1");
            humanoT = content.Load<Texture2D>("Texturas/humano-1");

            Sonido.PlayMusic(PiezasMusica.Menu);
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();
        }

        private void creditos_Selected(object sender, PlayerIndexEventArgs e) {
            ScreenManagerController.AddScreen(new MenuCreditos(), null);
        }

        private void NotImplemented(object sender, PlayerIndexEventArgs e) {
            PopupScreen notImplemented = new PopupScreen("Aun no implementado", "\nPresione <ESC> para volver", true);

            ScreenManagerController.AddScreen(notImplemented, null);
        }

        private void NivelSelected(object sender, PlayerIndexEventArgs e){
            Sonido.PlayMusic(PiezasMusica.CuevaIntro);
            LoadingScreen.Load(ScreenManagerController, false, string.Empty, null, new Nivel(this.SelectedEntry + 1));            
        }

        protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex) {
            Sonido.UnLoadContent();
            ScreenManagerController.Game.Exit();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            for (int i = 0; i < 5; i++) {
                tiempoTranscurridoG[i] += gameTime.ElapsedGameTime.Milliseconds;
                tiempoTranscurridoH[i] += gameTime.ElapsedGameTime.Milliseconds;

                if (tiempoTranscurridoG[i] > 1000f / 15) {
                    ++frameActualG[i];
                    tiempoTranscurridoG[i] = 0f;

                    if (frameActualG[i] >= 5) {
                        frameActualG[i] = 0;
                    }
                }

                if (tiempoTranscurridoH[i] > 1000f / 15) {
                    ++frameActualH[i];
                    tiempoTranscurridoH[i] = 0f;

                    if (frameActualH[i] >= 4) {
                        frameActualH[i] = 0;
                    }
                }

                spriteBatch.Draw(
                    goblinT,
                    new Vector2(startingPositionG + i * spacingPositionG, 500),
                    new Rectangle(frameActualG[i] * 15, 0, 15, 22),
                    Color.White * TransitionAlpha,
                    0f,
                    Vector2.Zero,
                    Vector2.One * 2,
                    SpriteEffects.FlipHorizontally,
                    0f
                );
                
                spriteBatch.Draw(
                    humanoT,
                    new Vector2(startingPositionH - i * spacingPositionH, 500),
                    new Rectangle(frameActualH[i] * 15, 0, 15, 22),
                    Color.White * TransitionAlpha,
                    0f,
                    Vector2.Zero,
                    Vector2.One * 2,
                    SpriteEffects.None,
                    0f
                );
            }            

            spriteBatch.End();
        }
    }
}
