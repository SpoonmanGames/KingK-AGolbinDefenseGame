using System;
using Microsoft.Xna.Framework;

namespace ScreenManager.MenuScren
{
    /// <summary>
    /// EventArgs que incluye el Index del player que ha lanzado este evento.
    /// Usado en el evento MenuEntry.Selected.
    /// </summary>
    public class PlayerIndexEventArgs : EventArgs
    {
        #region properties

        /// <summary>
        /// Obtiene el Index del player que gatilló este evento.
        /// </summary>
        public PlayerIndex PlayerIndex { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor que asigna el player index
        /// </summary>
        /// <param name="playerIndex">Index del player que ha generado un evento.</param>
        public PlayerIndexEventArgs(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        #endregion
    }
}
