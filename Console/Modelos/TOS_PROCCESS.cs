namespace Console.ServicioConsole
{
    public partial class TOS_PROCCESS
    {
        public string ImagenEstado
        {
            get
            {
                if (IS_OK)
                    return "/Imagenes/Finish.png";
                else
                    return "/Imagenes/Delete.png";
            }
        }
    }
}
