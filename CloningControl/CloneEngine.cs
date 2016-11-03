using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;
using System.Reflection;

namespace Fabio.SharpTools.CloningControl
{
    /// <summary>  
    /// The ControlCloneEngine creates copies of ASP.NET controls  
    /// </summary>  
    public sealed class CloneEngine
    {
        private static Object lockObject = new Object();

        private static int m_instanceCount;

        public CloneEngine(){ }

        /// <summary>  
        /// Clone a control.  Call this function in Page_Init  
        /// </summary>  
        /// <param name="ctrlSource">The control to clone</param>  
        /// <returns>a new copy of the control</returns>  
        public static System.Web.UI.Control Copy(System.Web.UI.Control ctrlSource)
        {
            lock (lockObject)
            {
                Type t = ctrlSource.GetType();

                System.Web.UI.Control ctrlDest = (System.Web.UI.Control)t.InvokeMember("", BindingFlags.CreateInstance, null, null, null);

                foreach (PropertyInfo prop in t.GetProperties())
                {
                    if (prop.CanWrite)
                    {
                        if (prop.Name == "ID" && ctrlSource.ID != null)
                        {
                            ctrlDest.ID = ctrlSource.ID + "c" + m_instanceCount;
                        }
                        else
                        {
                            prop.SetValue(ctrlDest, prop.GetValue(ctrlSource, null), null);
                        }
                    }
                }

                m_instanceCount++;

                return ctrlDest;
            }
        }

        /// <summary>  
        /// Clone a control.  Call this function in Page_Init  
        /// </summary>  
        /// <param name="ctrlSource">The control to clone</param>  
        /// <returns>a new copy of the control</returns>  
        public static HtmlControl Copy(HtmlControl ctrlSource)
        {
            lock (lockObject)
            {
                Type t = ctrlSource.GetType();

                HtmlControl ctrlDest = (HtmlControl)t.InvokeMember("", BindingFlags.CreateInstance, null, null, null);

                foreach (PropertyInfo prop in t.GetProperties())
                {
                    if (prop.CanWrite)
                    {
                        if (prop.Name == "ID" && ctrlSource.ID != null)
                        {
                            ctrlDest.ID = ctrlSource.ID + "c" + m_instanceCount;
                        }
                        else
                        {
                            prop.SetValue(ctrlDest, prop.GetValue(ctrlSource, null), null);
                        }
                    }
                }

                foreach (string key in ctrlSource.Attributes.Keys)
                {
                    ctrlDest.Attributes.Add(key, ctrlSource.Attributes[key]);
                }

                m_instanceCount++;

                return ctrlDest;
            }
        }
    }
}
