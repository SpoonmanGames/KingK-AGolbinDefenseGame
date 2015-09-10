using ImpulsaGameJamsAtari.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;
using System;
using System.Collections.Generic;

namespace ImpulsaGameJamsAtari.Forja {

    /// <summary>
    /// BUG: Si se entra antes que acabe la transición se queda pegado,
    /// aunque se puede volver al nivel presionando escape
    /// </summary>
    public class Forja : BaseMenuScreen {

        MenuEntry armaEntry;
        MenuEntry armaduraEntry;
        MenuEntry trampasEntry;

        public static int NivelArma = 0;
        int CostoNivelArma = 5;
        public static int NivelArmadura = 0;
        int CostoNivelArmadura = 10;
        public static int TrampasDisponibles = 0;
        int CostoTrampas = 30;

        Nivel Nivel;

        //grafico
        ContentManager content;
        Texture2D textura;
        Texture2D letrero;
        public Vector2 Posicion;
        public Vector2 Escala;
        Rectangle Sprite;
        List<Texture2D> iconos = new List<Texture2D>();

        public int Altura { get { return Sprite.Height * (int)Escala.Y; } }

        public int Ancho { get { return Sprite.Width * (int)Escala.X; } }

        public virtual Rectangle BoundingBox {
            get {
                return new Rectangle((int)Posicion.X, (int)Posicion.Y, Ancho, Altura);
            }
        }

        public Forja(Nivel nivel, string menuTitle) : base(menuTitle){
            this.Nivel = nivel;

            //Reinicia globales
            NivelArma = 0;
            NivelArmadura = 0;
            TrampasDisponibles = 0;

            armaEntry = new MenuEntry(this, string.Empty);
            armaduraEntry = new MenuEntry(this, string.Empty);
            trampasEntry = new MenuEntry(this, string.Empty);

            SetMenuEntryText();

            armaEntry.Selected += ArmaEntrySelected;
            armaduraEntry.Selected += ArmaduraEntrySelected;
            trampasEntry.Selected += TrampasEntrySelected;

            MenuEntries.Add(armaEntry);
            MenuEntries.Add(armaduraEntry);
            MenuEntries.Add(trampasEntry);

            Initialice();
        }

        void Initialice() {
            Posicion = Vector2.Zero;
            Escala = Vector2.One;
        }

        public void Load() {
            if (content == null)
                content = new ContentManager(Nivel.ScreenManagerController.Game.Services, "Content");

            this.textura = content.Load<Texture2D>("Texturas/forja-0");
            this.letrero = content.Load<Texture2D>("Texturas/forja");
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_herramientas"));
            iconos.Add(content.Load<Texture2D>("Texturas/Iconos/ico_valor"));
            this.Sprite = new Rectangle((int)Posicion.X, (int)Posicion.Y, this.textura.Width, this.textura.Height);
        }

        public void UnLoad() {
            content.Unload();
        }

        public void Dibujo() {
            SpriteBatch spriteBatch = Nivel.ScreenManagerController.SpriteBatch;

            spriteBatch.Draw(
                letrero,
                new Vector2(Posicion.X + textura.Width / 2, Posicion.Y - 30),
                new Rectangle(0,0, letrero.Width, letrero.Height),
                Color.White,
                0f,
                new Vector2(letrero.Width / 2, 0),
                Escala,
                SpriteEffects.None,
                0f
            );

            spriteBatch.Draw(
                textura,
                Posicion,
                Sprite,
                Color.White,
                0f,
                Vector2.Zero,
                Escala,
                SpriteEffects.None,
                0f
            );
        }

        void SetMenuEntryText() {
            armaEntry.Text = "Nivel Arma: " + NivelArma;
            armaduraEntry.Text = "Nivel Armadura: " + NivelArmadura;
            trampasEntry.Text = "Pinchos: " + TrampasDisponibles;
        }

        void ArmaEntrySelected(object sender, PlayerIndexEventArgs e) {
            if (Nivel.Herramientas >= CostoNivelArma) {
                Sonido.PlayEffect(Sound.PiezasEfectos.PowerUp);
                ++NivelArma;
                Nivel.Herramientas -= CostoNivelArma;
                SetMenuEntryText();
            }            
        }

        void ArmaduraEntrySelected(object sender, PlayerIndexEventArgs e) {
            if (Nivel.Herramientas >= CostoNivelArmadura) {
                Sonido.PlayEffect(Sound.PiezasEfectos.PowerUp);
                ++NivelArmadura;
                Nivel.Herramientas -= CostoNivelArmadura;
                SetMenuEntryText();
            }
        }

        void TrampasEntrySelected(object sender, PlayerIndexEventArgs e) {
            if (Nivel.Herramientas >= CostoTrampas) {
                Sonido.PlayEffect(Sound.PiezasEfectos.PowerUp);
                ++TrampasDisponibles;
                Nivel.Herramientas -= CostoTrampas;
                SetMenuEntryText();
            }
        }

        protected override void OnCancel(PlayerIndex playerIndex) {
            base.OnCancel(playerIndex);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime) {
            base.Draw(gameTime);

            string text = string.Empty;

            SpriteBatch spriteBatch = Nivel.ScreenManagerController.SpriteBatch;
            SpriteFont font = Nivel.ScreenManagerController.Font;

            if (this.SelectedEntry == 0) {
                text = CostoNivelArma.ToString();
            } else if (this.SelectedEntry == 1) {
                text = CostoNivelArmadura.ToString();
            } else {
                text = CostoTrampas.ToString();
            }

            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            //titulo
            Vector2 tituloPos = new Vector2(ScreenManagerController.GraphicsDevice.Viewport.Width / 2, 50);
            tituloPos.Y -= transitionOffset * 100;

            spriteBatch.Draw(
                letrero,
                tituloPos,
                new Rectangle(0, 0, letrero.Width, letrero.Height),
                Color.White * TransitionAlpha,
                0f,
                new Vector2(letrero.Width / 2, 0),
                Escala * 4,
                SpriteEffects.None,
                0f
            );

            //Costo herramientas
            Vector2 stringPosition = new Vector2();
            stringPosition.X = Nivel.ScreenManagerController.GraphicsDevice.Viewport.Width / 2 + 100 - iconos[1].Width;
            stringPosition.Y = Nivel.ScreenManagerController.GraphicsDevice.Viewport.Height / 2;

            if (ScreenState == ScreenManager.StateControl.ScreenState.TransitionOn)
                stringPosition.X -= transitionOffset * 256;
            else
                stringPosition.X += transitionOffset * 512;

            spriteBatch.Draw(
                iconos[1],
                stringPosition,
                new Rectangle(0, 0, iconos[1].Width, iconos[1].Height),
                Color.White * TransitionAlpha,
                0f,
                Vector2.Zero,
                Escala,
                SpriteEffects.None,
                0f
            );

            stringPosition.X += iconos[1].Height / 2;
            stringPosition.Y += iconos[1].Height + 10;
            Vector2 centerCA = new Vector2(font.MeasureString(text).X / 2, 0);

            spriteBatch.DrawString(
                font,
                text,
                stringPosition,
                Color.White * TransitionAlpha,
                0f,
                centerCA,
                1.5f,
                SpriteEffects.None,
                0f
            );

            //Herramientas actuales icono y texto
            stringPosition.X = Nivel.ScreenManagerController.GraphicsDevice.Viewport.Width / 2 - 100;
            stringPosition.Y = Nivel.ScreenManagerController.GraphicsDevice.Viewport.Height / 2;

            if (ScreenState == ScreenManager.StateControl.ScreenState.TransitionOn)
                stringPosition.X -= transitionOffset * 256;
            else
                stringPosition.X += transitionOffset * 512;

            spriteBatch.Draw(
                iconos[0],
                stringPosition,
                new Rectangle(0, 0, iconos[0].Width, iconos[0].Height),
                Color.White * TransitionAlpha,
                0f,
                Vector2.Zero,
                Escala,
                SpriteEffects.None,
                0f
            );

            stringPosition.X += iconos[0].Height / 2;
            stringPosition.Y += iconos[0].Height + 10;
            string herramientasA = Nivel.Herramientas.ToString();
            Vector2 centerHA = new Vector2(font.MeasureString(herramientasA).X / 2, 0);

            spriteBatch.DrawString(
                font,
                herramientasA,
                stringPosition,
                Color.White * TransitionAlpha,
                0f,
                centerHA,
                1.5f,
                SpriteEffects.None,
                0f
            );

            spriteBatch.End();
        }
    }
}
