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
using PdfTagger.Dat.Txt;
using PdfTagger.Pdf;
using System.Collections.Generic;

namespace PdfTagger.Dat
{

    /// <summary>
    /// Resultado de comparación de un pdf.
    /// </summary>
    public class PdfCompareResult
    {

        #region Private Mambers Variables

        PdfUnstructuredDoc _Pdf;
        IMetadata _Metadata;
        IHierarchySet _HierarchySet;

        #endregion

        #region Public Properties

        /// <summary>
        /// Categoría de documento a la que pertenece el pdf.
        /// </summary>
        public string DocCategory
        {
            get
            {
                return _Pdf.DocCategory;
            }
        }

        /// <summary>
        /// ID del documento pdf.
        /// </summary>
        public string DocID
        {
            get
            {
                return _Pdf.DocID;
            }
        }

        /// <summary>
        /// Nombre del catálogo de jerarquías
        /// utilizado.
        /// </summary>
        public string HierarchySetName
        {
            get
            {
                if (_HierarchySet == null)
                    return null;

                return _HierarchySet.GetType().FullName;
            }
        }

        /// <summary>
        /// Clase que implementa la interfaz IMetadata
        /// asociada al resultado de identificación de
        /// patrones.
        /// utilizado.
        /// </summary>
        public string MetadataName
        {
            get
            {
                if (_Metadata == null)
                    return null;

                return _Metadata.GetType().FullName;
            }
        }

        /// <summary>
        /// Información de coincidencias encontradas en los 
        /// datos no estructurados contenidos en los rectángulos 
        /// con información  de grupos relacionados
        /// de palabras obtenidos de la página.
        /// </summary>
        public List<PdfCompareInfo> WordGroupsInfos { get; set; }

        /// <summary>
        /// Información de coincidencias encontradas en los 
        /// rectángulos con información  del texto por
        /// líneas obtenidos de la página.
        /// </summary>
        public List<PdfCompareInfo> LinesInfos { get; set; }

        /// <summary>
        /// Información de coincidencias encontradas en la 
        /// información no estructurada total de la 
        /// página de pdf en forma de texto.
        /// </summary>
        public List<PdfCompareInfo> PdfTextInfos { get; set; }

        /// <summary>
        /// Información de coincidencias encontradas en los 
        /// datos no estructurados contenidos en los rectángulos 
        /// con información  de grupos relacionados
        /// de palabras obtenidos de la página;
        /// además del color, tamaño y nombre de la fuente.
        /// </summary>
        public List<PdfCompareInfo> ColorFontWordGroupsInfos { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de PdfCompareResult.
        /// </summary>
        public PdfCompareResult()
        {
            WordGroupsInfos = new List<PdfCompareInfo>();
            LinesInfos = new List<PdfCompareInfo>();
            PdfTextInfos = new List<PdfCompareInfo>();
            ColorFontWordGroupsInfos = new List<PdfCompareInfo>();
        }

        /// <summary>
        /// Construye una nueva instancia de PdfCompareResult.
        /// </summary>
        /// <param name="pdf">PdfUnstructuredDoc a partir del cual se ha obtenido.</param>
        /// <param name="metadata">IMetadata a partir del cual se ha obtenido.</param>
        /// <param name="hierarchySet">Catálogo de jerarquías por tipo.</param>
        public PdfCompareResult(PdfUnstructuredDoc pdf, IMetadata metadata, 
            IHierarchySet hierarchySet) : this()
        {
            _Pdf = pdf;
            _Metadata = metadata;
            _HierarchySet = hierarchySet;
        }

        #endregion

    }
}
