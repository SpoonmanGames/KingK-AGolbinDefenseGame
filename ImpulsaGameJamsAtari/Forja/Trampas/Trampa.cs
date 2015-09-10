using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ImpulsaGameJamsAtari.Forja.Trampas {
    public abstract class Trampa {

        Texture2D textura;
        Rectangle sprite;
        string nombreTextura;
        ContentManager content;
        
        protected Nivel Nivel;

        public Vector2 Posicion { get; set; }

        public Vector2 Escala { get { return this.vectorEscala; } set { this.vectorEscala = value; } }
        Vector2 vectorEscala;

        public int Altura { get { return sprite.Height * (int)Escala.Y; } }

        public int Ancho { get { return sprite.Width * (int)Escala.X; } }

        public int ContadorMuertes { get; set; }

        public virtual Rectangle BoundingBox {
            get {
                    return new Rectangle((int)Posicion.X, (int)Posicion.Y, Ancho, Altura);
            }
        }

        public Trampa(Nivel nivel, string nombreTextura) {
            this.nombreTextura = nombreTextura;
            this.Escala = Vector2.One;
            this.Posicion = Vector2.Zero;
            this.ContadorMuertes = 0;
        }

        public virtual void LoadContent(bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null) {
            if (content == null && !tutorial)
                content = new ContentManager(Nivel.ScreenManagerController.Game.Services, "Content");
            else
                content = new ContentManager(screenManagerController.Game.Services, "Content");

            textura = content.Load<Texture2D>(string.Format("Texturas/{0}", this.nombreTextura));
            sprite = new Rectangle(0, 0, textura.Width, textura.Height);
        }

        public virtual void UnloadContent() {
            content.Unload();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime, bool tutorial = false, ScreenManager.ScreenManager screenManagerController = null, float transitionAlpha = 1f) {
            SpriteBatch spriteBatch;

            if(!tutorial)
                spriteBatch = Nivel.ScreenManagerController.SpriteBatch;
            else
                spriteBatch = screenManagerController.SpriteBatch;

            spriteBatch.Draw(
                textura,
                Posicion,
                sprite,
                Color.White * transitionAlpha,
                0f,
                Vector2.Zero,
                vectorEscala,
                SpriteEffects.None,
                0f
            );
        }
    }
}
