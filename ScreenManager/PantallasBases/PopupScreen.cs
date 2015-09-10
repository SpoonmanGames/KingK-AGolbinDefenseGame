using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ScreenManager.MenuScren;
using ScreenManager.StateControl;
using System;

namespace ScreenManager.PantallasBases
{
    /// <summary>
    /// Clase usada como Popup Screen, puede llevar diferentes tipos de mensaje.
    /// Usualmente del tipo "¿Estás seguro?"
    /// </summary>
    public class PopupScreen : GameScreen
    {
        #region Fields


        /// <summary>
        /// Mensaje a mostrar en la ventana
        /// </summary>
        string message;

        /// <summary>
        /// Mensaje usado para mostrar las opciones de controles
        /// </summary>
        string usageText;

        /// <summary>
        /// Textura usada para el fondo del Popup
        /// </summary>
        Texture2D gradientTexture;

        #endregion

        #region Events


        /// <summary>
        /// Evento para cuando la opción es aceptada
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Accepted;


        /// <summary>
        /// Evento para cuando la opción es cancelada
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Constructores


        /// <summary>
        /// Constructor que automaticamente incluye las opciones "A=Sí, B=Cancel"
        /// en conjunto con el texto mostrado.
        /// </summary>
        /// <param name="message">Mensaje a mostrar en la ventana.</param>
        public PopupScreen(string message)
            : this(message,
                   "\nTecla <Enter> = Sí" +
                   "\nTecla <Esc> = Cancel",
                   true)
        { }


        /// <summary>
        /// Permite elegir si se incluye "A=Sí, B=Cancel" o no.
        /// </summary>
        /// <param name="message">Mensaje a mostrar en la ventana</param>
        /// <param name="usageText">Texto que dice que hacer tras el PopUp</param>
        /// <param name="includeUsageText">Permite determinar si se hará uso del texto de opciones por defecto o no</param>
        public PopupScreen(string message, string usageText, bool includeUsageText)
        {
            // Texto de uso
            this.usageText = usageText;

            if (includeUsageText)
                this.message = message + usageText;
            else
                this.message = message;

            // Asigna la Screen como un Popup
            IsAPopup = true;

            // Asigna los tiepos de transición
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        #endregion

        #region Load Content

        /// <summary>
        /// Ya que las ventanas popup pueden ser llamadas muchas veces, el contenido cargado por ellas
        /// es cargado en el Content compartido por la clase Game, por lo que la carga realizada
        /// se mantendrá por siempre. De este modo, si se vuelve a cargar este mismo content solo 
        /// se recivira una referencia del contenido cargado, evitando sobrecargas.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManagerController.Game.Content;

            gradientTexture = content.Load<Texture2D>("Gradientes/PopupGradient");
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Como es una Screen puede responder a los controles de los usuarios, este
        /// metodo permite la detección del teclado.
        /// </summary>
        /// <param name="input">Input que tiene el mapeo del teclado.</param>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;

            /* Pasamos el ControllingPlauer como nulo si queremos que cualquier player
             * realice acciones, o con un valor para el plauer que deseamos que 
             * realice la acción.
             * Luego playerIndex guardará la información de quien hizo la acción.
             */
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                // Levanta el evento Accepted y se sale del Popup
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Levanta el evento Cancelled y se sale del Popup
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Dibuja la Screen
        /// </summary>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public override void Draw(GameTime gameTime)
        {
            // Adquiere el batch y el font del ScreenManager
            SpriteBatch spriteBatch = ScreenManagerController.SpriteBatch;
            SpriteFont font = ScreenManagerController.Font;

            //Oscurece todas las Screen que hayan quedado detras del Popup
            ScreenManagerController.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            //Centra el Popup en la Pantalla (Viewport)
            Viewport viewport = ScreenManagerController.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // El borde del rectangulo dond estará el texto es un poco más grande que el texto mismo.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Como toda screen, esta tiene transición de entrada y salida
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);

            spriteBatch.Draw(gradientTexture, backgroundRectangle, color);
            spriteBatch.DrawString(font, message, textPosition, color);

            spriteBatch.End();
        }


        #endregion
    }
}
