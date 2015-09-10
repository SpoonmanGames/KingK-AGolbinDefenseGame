using ImpulsaGameJamsAtari.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;
using ScreenManager.PantallasBases;
using System.Collections.Generic;
using System.Linq;

namespace ImpulsaGameJamsAtari.Menus {

    public class PresentacionLogo : BaseMenuScreen {

        float waitDelay;
        float maxWaitDelay;

        Texture2D fondo;
        List<string> nombreFondo = new List<string>();
        int indice;
        bool done;

        ContentManager content;

        public PresentacionLogo(int indice, params string[] nombreFondo) : base(string.Empty) {
            this.nombreFondo = nombreFondo.ToList<string>();
            this.indice = indice;
            this.waitDelay = 0f;
            this.maxWaitDelay = 3000f;
            this.done = false;
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            fondo = content.Load<Texture2D>(string.Format("Texturas/{0}", nombreFondo[indice]));

            if (!Sonido.Creado) {
                Sonido s = new Sonido(ScreenManagerController);
                s.LoadContent();
            }

            if(indice == 0)
                Sonido.PlayEffect(Sound.PiezasEfectos.SPIntro);
            else
                Sonido.PlayEffect(Sound.PiezasEfectos.IGIntro);
        }

        public override void UnloadContent() {
            base.UnloadContent();

            content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            waitDelay += gameTime.ElapsedGameTime.Milliseconds;

            if (waitDelay >= maxWaitDelay && !done) {
                this.done = true;
                ++indice;
                if(indice != nombreFondo.Count){
                    ScreenManagerController.AddScreen(new PresentacionLogo(indice, nombreFondo.ToArray<string>()), null);
                }else{
                    ScreenManagerController.AddScreen(new BackgroundScreen("Texturas/mainmenu"), null);
                    ScreenManagerController.AddScreen(new MenuPrincipal(string.Empty), null);
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            base.Draw(gameTime);

            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            spriteBatch.Draw(
                fondo,
                new Rectangle(0, 0, ScreenManagerController.GraphicsDevice.Viewport.Width, ScreenManagerController.GraphicsDevice.Viewport.Height),
                Color.White * TransitionAlpha
            );

            spriteBatch.End();
        }

        protected override void OnCancel(PlayerIndex playerIndex) {
            
        }
    }
}
