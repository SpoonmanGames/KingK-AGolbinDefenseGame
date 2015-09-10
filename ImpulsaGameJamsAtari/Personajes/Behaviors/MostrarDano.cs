namespace ImpulsaGameJamsAtari.Personajes.Behaviors
{
    public class MostrarDano
    {
        public bool IsInverso;
        public float Alpha;
        public float Velocidad;
        public float DanoOutputTime;
        public float DanoTransition;
        public bool Terminado;

        public string GetDano{ get { return this.dano; }}
        string dano;

        public MostrarDano(string dano, float velocidad)
        {
            this.Terminado = false;
            this.IsInverso = false;
            this.Alpha = 0;
            this.DanoOutputTime = 0;
            this.DanoTransition = 0;
            this.Velocidad = velocidad;

            this.dano = dano;
        }
    }
}
