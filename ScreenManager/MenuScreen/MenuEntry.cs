using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ScreenManager.MenuScren
{
    /// <summary>
    /// Clase que representa un único entry en una pantalla de menu. Por defecto
    /// solo dibuja un texto, pero esto se puede custumizar. También usa un evento
    /// que se activa cuando el entry es seleccionado.
    /// </summary>
    public class MenuEntry
    {
        #region Properties


        /// <summary>
        /// Obtiene y asigna el texto que será renderizado en este entry
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// Obtiene y asigna la posición en la que será dibujado el entry.
        /// Esto es actualizado cada vez que update es llamado
        /// </summary>
        public Vector2 Position { get; set; }


        /// <summary>
        /// Guarda un acceso a la Screen en la que este entry toma lugar
        /// </summary>
        internal BaseMenuScreen ScreenController { get; set; }


        /// <summary>
        /// Obtiene el espacio en altura que requiere este entry según el texto
        /// </summary>
        public int TextHeight
        {
            get { return ScreenController.ScreenManagerController.Font.LineSpacing; }
        }


        /// <summary>
        /// Obtiene el espacio en anchura que requiere este entry según el texto. Sirve para centrarlo.
        /// </summary>
        public int TextWidth
        {
            get { return (int)ScreenController.ScreenManagerController.Font.MeasureString(Text).X; }
        }


        /// <summary>
        /// Realiza un efecto de FADE durante la selección del entry.
        /// </summary>
        /// <remarks>
        /// La entry sale de la transición cuando es deseleccionada
        /// </remarks>
        private float SelectionFade { get; set; }



        #endregion

        #region Events

        /// <summary>
        /// El evento se activa cuando el MenuEntry es seleccionado en forma de avance.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> ForwardSelected;

        /// <summary>
        /// El evento se activa cuando el MenuEntry es seleccionado.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// El evento se activa cuando el MenuEntry es seleccionado en forma inversa.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> ReverseSelected;

        /// <summary>
        /// Methodo para activar el Selected Event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        /// <summary>
        /// Methodo para activar el ReverseSelected Event.
        /// </summary>
        protected internal virtual void OnReverseSelectEntry(PlayerIndex playerIndex)
        {
            if (ReverseSelected != null)
                ReverseSelected(this, new PlayerIndexEventArgs(playerIndex));
        }

        /// <summary>
        /// Methodo para activar el ReverseSelected Event.
        /// </summary>
        protected internal virtual void OnForwardSelectEntry(PlayerIndex playerIndex)
        {
            if (ForwardSelected != null)
                ForwardSelected(this, new PlayerIndexEventArgs(playerIndex));
        }

        #endregion

        #region Constructor


        /// <summary>
        /// Crea un meny entry con el texto especificado
        /// </summary>
        /// <param name="baseMenuScreen">Screen donde toma contexto esta entry</param>
        /// <param name="text">Texto a escribir en el entry</param>
        public MenuEntry(BaseMenuScreen baseMenuScreen, string text)
        {
            ScreenController = baseMenuScreen;
            Text = text;
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Mantiene actualizada la menu entry en el juego
        /// </summary>
        /// <param name="isSelected">Booleano para saber si está seleccionada o no</param>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public virtual void Update(bool isSelected, GameTime gameTime)
        {
            /* Cuando la selección del menú cambia, el entry gradualmente realiza
             * un efecto de fade entre su estado de seleccionado y no seleccionado,
             * creando un efect más suave al pasar ente estos dos estados
             */
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                SelectionFade = Math.Min(SelectionFade + fadeSpeed, 1);
            else
                SelectionFade = Math.Max(SelectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Dibuja el entry. Puede realizarce un override de este metodo para mejorar la apariencia
        /// </summary>
        /// <param name="isSelected">Booleano para saber si está seleccionada o no</param>
        /// <param name="gameTime">Para obtener el tiempo del juego</param>
        public virtual void Draw(bool isSelected, GameTime gameTime)
        {
            //Si está seleccionada le da un color amarillo, si no, es blanco
            Color color = isSelected ? Color.Yellow : Color.White;

            //Permite cambiar el tamaño del entry cuando ha sido seleccionado
            //También le da un moviento sinusoidal
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * SelectionFade;

            //Modifica el alpha del entry de acuerdo al estado de transición del Screen al que pertenece
            color *= ScreenController.TransitionAlpha;

            //Dibuja el texto centrado en la Screen
            ScreenManager screenManager = ScreenController.ScreenManagerController;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;

            Vector2 origin = new Vector2(0, TextHeight / 2);

            spriteBatch.DrawString(font, Text, Position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        #endregion
    }
}
