using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;
using System;
using System.Collections.Generic;

namespace ImpulsaGameJamsAtari.Menus {
    class MenuCreditos : BaseMenuScreen {

        List<string> creditos = new List<string>();
        ContentManager content;
        Texture2D fondo;

        public MenuCreditos()
            : base("Créditos") {

                creditos.Add("~~Spoonman Games~~");
                creditos.Add("Creado y dirigido por Esteban Gaete");                
                creditos.Add(string.Empty);
                creditos.Add("~~Animaciones~~");
                creditos.Add("Don Calaca");
                creditos.Add(string.Empty);
                creditos.Add("~~Música~~");
                creditos.Add("Nikolai M. Henríquez");
                creditos.Add(string.Empty);
                creditos.Add("~~Creado para la Game Jam~~");
                creditos.Add("Impulsa Games Jams #IG2A43");
                creditos.Add(string.Empty);
                creditos.Add("~~Agradecimientos a~~");
                creditos.Add("Impulsa Games por hacer la  Game Jam");
                creditos.Add("Kuro Powah Stein, Jack Esneik y Victor Gaete");
                creditos.Add("Por probar el juego en su estado BETA");
                creditos.Add("A la comunidad del foro de Impulsa Games por el apoyo :)");
                creditos.Add(string.Empty);
                creditos.Add("Gracias a todos!!");
                creditos.Add(string.Empty);
                creditos.Add("www.spoonmangames.cl");
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManagerController.Game.Services, "Content");

            fondo = content.Load<Texture2D>("Texturas/blank");
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
                new Rectangle(
                    0,
                    0,
                    ScreenManagerController.GraphicsDevice.Viewport.Width,
                    ScreenManagerController.GraphicsDevice.Viewport.Height),
                Color.Black * TransitionAlpha
            );

            spriteBatch.End();

            base.Draw(gameTime);            

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            for (int i = 0; i < creditos.Count; i++){
                int center = ScreenManagerController.GraphicsDevice.Viewport.Width / 2;
                int originS = (int)font.MeasureString(creditos[i]).X / 2;
                Vector2 posicion = new Vector2(center, 130 + 20 * i);

                posicion.Y -= transitionOffset * 100;

                spriteBatch.DrawString(
                    font,
                    creditos[i],
                    posicion,
                    Color.White * TransitionAlpha,
                    0f,
                    new Vector2(originS, 0),
                    1f,
                    SpriteEffects.None,
                    0f
                );   
            }           

            spriteBatch.End();
        }
    }
}
