//------------------------------------------------------------------------------
// <copyright file="VSTodoListPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using VSToDoList.BL.Helpers;

namespace VSToDoList
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "0.2.2", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(VSTodoListPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(VSToDoList.UI.MainWindow.ToDoListWindow))]
    //[ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    //[ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.SolutionExists_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class VSTodoListPackage : Package
    {
        /// <summary>
        /// VSTodoListPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "d8b2716d-918d-47ad-8938-706d983d9d6c";

        /// <summary>
        /// Initializes a new instance of the <see cref="VSTodoListPackage"/> class.
        /// </summary>
        public VSTodoListPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            ConfigureJsonSerializer();
            VSToDoList.UI.MainWindow.ToDoListWindowCommand.Initialize(this);

            // Add the Service provider to the common instances for later reuse
            ApplicationCommons.Instances.ServiceProvider = this;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // If the package is being unloaded or Visual Studio is closing, unregister from Solution events
            if (ApplicationCommons.Instances.SolutionService != null && ApplicationCommons.Instances.SolutionServiceCookie != 0)
            {
                ApplicationCommons.Instances.SolutionService.UnadviseSolutionEvents(ApplicationCommons.Instances.SolutionServiceCookie);
                ApplicationCommons.Instances.SolutionService = null;
            }
        }

        private void ConfigureJsonSerializer()
        {
            var jsonConfig = new JsonSerializerSettings();
            jsonConfig.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonConfig.Formatting = Formatting.Indented;
            jsonConfig.TypeNameHandling = TypeNameHandling.Auto;
            JsonConvert.DefaultSettings = () => jsonConfig;
        }

        #endregion Package Members
    }
}