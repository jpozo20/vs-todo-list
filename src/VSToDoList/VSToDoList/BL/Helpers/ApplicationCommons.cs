using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace VSToDoList.BL.Helpers
{
    /// <summary>
    /// A static class to hold global useful variables in order to avoid rountrips and dependency hell
    /// </summary>
    internal static class ApplicationCommons
    {
        /// <summary>
        /// Will hold common use instaces like ServiceProvider, SolutionService, EnvDTE,etc
        /// </summary>
        internal static class Instances
        {
            /// <summary>
            /// This service gives access to the solution and projects information.
            /// </summary>
            internal static IServiceProvider ServiceProvider { get; set; }

            /// <summary>
            /// This service gives access to the solution events.
            /// </summary>
            internal static IVsSolution SolutionService { get; set; }

            /// <summary>
            /// A number (id?) that allows Visual Studio to unregister our event listener
            /// </summary>
            internal static uint SolutionServiceCookie { get; set; }
        }

        internal static class Services
        {
            /// <summary>
            /// Gets the <see cref="EnvDTE"/> service. 
            /// This service gives access to the solution and projects information.
            /// </summary>
            /// <returns></returns>
            internal static EnvDTE.DTE GetEnvDTE()
            {
                EnvDTE.DTE dte = null;
                if(Instances.ServiceProvider == null)
                {
                    dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                    return dte;
                }

                dte = Instances.ServiceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                return dte;
            }

            /// <summary>
            /// Gets the <see cref="IVsSolution"/> service. 
            /// This service gives access to the solution events.
            /// </summary>
            /// <returns></returns>
            internal static IVsSolution GetVsSolutionService()
            {
                IVsSolution _solutionService;
                if (Instances.ServiceProvider == null)
                {
                    _solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                    return _solutionService;
                }

                _solutionService = Instances.ServiceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
                return _solutionService;
            }
        }
    }
}