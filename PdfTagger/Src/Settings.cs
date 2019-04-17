/*
    This file is part of the PdfTagger (R) project.
    Copyright (c) 2017-2018 Irene Solutions SL
    Authors: Irene Solutions SL.

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    IRENE SOLUTIONS SL. IRENE SOLUTIONS SL DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
    OF THIRD PARTY RIGHTS
    
    This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
        http://pdftagger.com/terms-of-use.pdf
    
    The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
    
    You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the PdfTagger software without
    disclosing the source code of your own applications.
    These activities include: offering paid services to customers as an ASP,
    serving extract PDFs data on the fly in a web application, shipping PdfTagger
    with a closed source product.
    
    For more information, please contact Irene Solutions SL. at this
    address: info@irenesolutions.com
 */
using PdfTagger.Xml;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PdfTagger
{
    /// <summary>
    /// Contiene la configuración del sistema. Se almacena
    /// en ProgramData\PdfTagger en el archivo settings.xml.
    /// </summary>
    [Serializable]
    [XmlRoot("PdfTagger")]
    public class Settings
    {

        #region Private Member Variables 

        /// <summary>
        /// Parth separator win="\" and linux ="/".
        /// </summary>
        static char _PathSep = System.IO.Path.DirectorySeparatorChar;

        /// <summary>
        /// Configuración actual.
        /// </summary>
        static Settings _Current;

        /// <summary>
        /// Ruta al directorio de configuración.
        /// </summary>
        internal static string Path = System.Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData) + $"{_PathSep}PdfTagger{_PathSep}";


        /// <summary>
        /// Nombre del fichero de configuración.
        /// </summary>
        internal static string FileName = "settings.xml";

        #endregion

        #region Private Methods

        /// <summary>
        /// Inicia estaticos.
        /// </summary>
        /// <returns></returns>
        internal static Settings Get()
        {

            string FullPath = $"{Path}{_PathSep}{FileName}";
          
            if (File.Exists(FullPath))
            {
                _Current = XmlParser.FromXml<Settings>(FullPath);
            }
            else
            {

                _Current = new Settings();

                _Current.PatternsPath = Path + $"Patterns{_PathSep}";
                _Current.MaxPatternCount = 1500;
                _Current.MinRectangleCommon = 0.95f;

            }

            CheckDirectories();

            return _Current;
        }

        /// <summary>
        /// Aseguro existencia de directorios de trabajo.
        /// </summary>
        private static void CheckDirectories()
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            if (!Directory.Exists(_Current.PatternsPath))
                Directory.CreateDirectory(_Current.PatternsPath);

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor estático de la clase Settings.
        /// </summary>
        static Settings()
        {
            Get();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Configuración en curso.
        /// </summary>
        public static Settings Current
        {
            get
            {
                return _Current;
            }
            set
            {
                _Current = value;
            }
        }

        /// <summary>
        /// Ruta al directorio donde se encuentran
        /// los patrones de búsqueda.
        /// </summary>
        [XmlElement("PatternsPath")]
        public string PatternsPath { get; set; }

        /// <summary>
        /// Número de patrones a almacenar como
        /// máximo.
        /// </summary>
        [XmlElement("MaxPatternCount")]
        public int MaxPatternCount { get; set; }

        /// <summary>
        /// Coeficiente mínimo de área
        /// compartida, utilizado en la selección de 
        /// rectángulos en patrones.
        /// </summary>
        [XmlElement("MinRectangleCommon")]
        public float MinRectangleCommon { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Guarda la configuración en curso actual.
        /// </summary>
        public static void Save()
        {

            CheckDirectories();

            string FullPath = $"{Path}{_PathSep}{FileName}";

            XmlParser.SaveAsXml(Current, FullPath);           

        }

        /// <summary>
        /// Esteblece el archivo de configuración con el cual trabajar.
        /// </summary>
        /// <param name="fileName">Nombre del archivo de configuración a utilizar.</param>
        public static void SetFileName(string fileName)
        {
            FileName = fileName;
            Get();
        }

        #endregion

    }
}
