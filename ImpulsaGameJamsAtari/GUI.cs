using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ImpulsaGameJamsAtari {
    public class GUI {
        List<int> guiOutput = new List<int>();
        List<Texture2D> iconos = new List<Texture2D>();

        ContentManager content;
        Nivel Nivel;

        public GUI(Nivel nivel) {
            this.Nivel = nivel;
        }

        public void Update(params int[] estadisticas){
            guiOutput = estadisticas.ToList<int>();            
        }

        public void LoadContent() {
            if (content == null)
                content = new ContentManager(Nivel.ScreenManagerController.Game.Services, "Content");

            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_herramientas"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_dano"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_armor"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_trampas"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_muertes"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_oleada"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_tiempo"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_nivel"));
        }

        public void UnloadContent() {
            content.Unload();
        }

        public void Draw() {
            SpriteBatch spriteBatch = Nivel.ScreenManagerController.SpriteBatch;
            SpriteFont font = Nivel.ScreenManagerController.Font;

            for (int i = 0; i < iconos.Count; i++){
                spriteBatch.Draw(
                    iconos[i],
                    new Vector2(10 + i * (iconos[i].Width + 20), 10),
                    Color.White
                );
            }

            for (int i = 0; i < guiOutput.Count; i++) {
                string output = guiOutput[i].ToString();

                if (i == 5) {
                    output += "/5";
                }

                int center = (int)font.MeasureString(output).X / 2;


                spriteBatch.DrawString(
                    font,
                    output,
                    new Vector2(
                        10 + (iconos[0].Width / 2) + i * (iconos[0].Width + 20),
                        iconos[0].Height + 20),
                    Color.White,
                    0f,
                    new Vector2(
                        center,
                        0),
                    1.5f,
                    SpriteEffects.None,
                    0f
                );
            }
            
            
        }


    }
}
