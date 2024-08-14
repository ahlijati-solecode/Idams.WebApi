using System.Reflection;

namespace Idams.Core.Extenstions
{
    public static class CopyExtension
    {
        public static void CopyProperties(this object source, object destination)
        {
            // Iterate the Properties of the destination instance and  
            // populate them from their source counterparts  
            PropertyInfo[] destinationProperties = destination.GetType().GetProperties();
            foreach (PropertyInfo destinationPi in destinationProperties)
            {
                PropertyInfo sourcePi = source.GetType().GetProperty(destinationPi.Name);
                if(sourcePi != null && sourcePi.GetValue(source, null) != null)
                {
                    if(sourcePi.PropertyType == typeof(int))
                    {
                        if(Convert.ToInt32((int)sourcePi.GetValue(source, null)) != 0)
                        {
                            destinationPi.SetValue(destination, sourcePi.GetValue(source, null), null);
                        }
                    }
                    else destinationPi.SetValue(destination, sourcePi.GetValue(source, null), null);
                }
            }
        }
    }
}
