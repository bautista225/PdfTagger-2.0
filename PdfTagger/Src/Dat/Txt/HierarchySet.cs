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
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PdfTagger.Dat.Txt
{

    /// <summary>
    /// Catálogo de jerarquías por tipo a aplicar en una operación
    /// de comparación determinada.
    /// </summary>
    public class HierarchySet : IHierarchySet
    {

        #region Constructors

        /// <summary>
        /// Construye una nueva instancia de un catálogo
        /// de jerarquías.
        /// </summary>
        public HierarchySet()
        {
            HierarchyByType = new Dictionary<Type, ITextParserHierarchy>();
            HierarchyByPropertyName = new Dictionary<string, ITextParserHierarchy>();
        }

        #endregion

        #region public Properties

        /// <summary>
        /// Diccionario de Jararquías según el tipo
        /// de datos.
        /// </summary>
        protected Dictionary<Type, ITextParserHierarchy> HierarchyByType;

        /// <summary>
        /// Diccionario de Jararquías según el nombre de la propiedad.
        /// de datos.
        /// </summary>
        protected Dictionary<string, ITextParserHierarchy> HierarchyByPropertyName;

        #endregion    

        #region Public Methods

        /// <summary>
        /// Devuelve la jerarquía de analisis de texto
        /// aplicable aun tipo determinado en este catálogo
        /// de jearaquía.
        /// </summary>
        /// <param name="pInf">PropertyInfo para el cual devolver el
        /// catálogo de jerarquías.</param>
        /// <returns></returns>
        public ITextParserHierarchy GetParserHierarchy(PropertyInfo pInf)
        {

            if(HierarchyByPropertyName.ContainsKey(pInf.Name))
                return HierarchyByPropertyName[pInf.Name];

            if (!HierarchyByType.ContainsKey(pInf.PropertyType))
                return null;

            return HierarchyByType[pInf.PropertyType];
        }   

        #endregion

    }
}
