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

namespace PdfTagger.Dat.Met.Bus
{

    /// <summary>
    /// Representa una entidad de datos estructurados
    /// extraídos de un pdf de factura.
    /// </summary>
    public class InvoiceMetadata : Metadata
    {

        #region Public Properties

        /// <summary>
        /// Número de factura.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// serie de factura.
        /// </summary>
        public string InvoiceSerie { get; set; }

        /// <summary>
        /// Fecha de emisión.
        /// </summary>
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Pedido de compras.
        /// </summary>
        public string PurchaseOrder { get; set; }

        /// <summary>
        /// Albarán de entrega.
        /// </summary>
        public string Delivery { get; set; }

        /// <summary>
        /// NIF de la parte vendedora.
        /// </summary>
        public string SellerPartyID { get; set; }

        /// <summary>
        /// NIF de la parte compradora.
        /// </summary>
        public string BuyerPartyID { get; set; }

        /// <summary>
        /// Total factura.
        /// </summary>
        public decimal GrossAmount { get; set; }

        /// <summary>
        /// Código de moneda.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Base de impuestos soportados 1.
        /// </summary>
        public decimal TaxesOutputsBase01 { get; set; }

        /// <summary>
        /// Base de impuestos soportados 2.
        /// </summary>
        public decimal TaxesOutputsBase02 { get; set; }

        /// <summary>
        /// Base de impuestos soportados 3.
        /// </summary>
        public decimal TaxesOutputsBase03 { get; set; }

        /// <summary>
        /// Base de impuestos soportados 4.
        /// </summary>
        public decimal TaxesOutputsBase04 { get; set; }

        /// <summary>
        /// Tipo impositivo de impuestos soportados 1.
        /// </summary>
        public decimal TaxesOutputsRate01 { get; set; }

        /// <summary>
        /// Tipo impositivo de impuestos soportados 2.
        /// </summary>
        public decimal TaxesOutputsRate02 { get; set; }

        /// <summary>
        /// Tipo impositivo de impuestos soportados 3.
        /// </summary>
        public decimal TaxesOutputsRate03 { get; set; }

        /// <summary>
        /// Tipo impositivo de impuestos soportados 4.
        /// </summary>
        public decimal TaxesOutputsRate04 { get; set; }

        /// <summary>
        /// Cuota de impuestos soportados 1.
        /// </summary>
        public decimal TaxesOutputsAmount01 { get; set; }

        /// <summary>
        /// Cuota de impuestos soportados 2.
        /// </summary>
        public decimal TaxesOutputsAmount02 { get; set; }

        /// <summary>
        /// Cuota de impuestos soportados 3.
        /// </summary>
        public decimal TaxesOutputsAmount03 { get; set; }

        /// <summary>
        /// Cuota de impuestos soportados 4.
        /// </summary>
        public decimal TaxesOutputsAmount04 { get; set; }

        /// <summary>
        /// Base de impuestos retenidos 1.
        /// </summary>
        public decimal TaxesWithholdingBase01 { get; set; }

        /// <summary>
        /// Tipo de impuestos retenidos 1.
        /// </summary>
        public decimal TaxesWithholdingRate01 { get; set; }

        /// <summary>
        /// Cuota de impuestos retenidos 1.
        /// </summary>
        public decimal TaxesWithholdingAmount01 { get; set; }

        /// <summary>
        /// Texto línea factura 1.
        /// </summary>
        public string InvoiceLineText01 { get; set; }

        /// <summary>
        /// Texto línea factura 2.
        /// </summary>
        public string InvoiceLineText02 { get; set; }

        /// <summary>
        /// Texto línea factura 3.
        /// </summary>
        public string InvoiceLineText03 { get; set; }

        /// <summary>
        /// Texto línea factura 4.
        /// </summary>
        public string InvoiceLineText04 { get; set; }

        /// <summary>
        /// Texto línea factura 5.
        /// </summary>
        public string InvoiceLineText05 { get; set; }

        /// <summary>
        /// Texto línea factura 6.
        /// </summary>
        public string InvoiceLineText06 { get; set; }

        /// <summary>
        /// Texto línea factura 7.
        /// </summary>
        public string InvoiceLineText07 { get; set; }

        /// <summary>
        /// Texto línea factura 8.
        /// </summary>
        public string InvoiceLineText08 { get; set; }

        /// <summary>
        /// Texto línea factura 9.
        /// </summary>
        public string InvoiceLineText09 { get; set; }

        /// <summary>
        /// Texto línea factura 10.
        /// </summary>
        public string InvoiceLineText10 { get; set; }

        /// <summary>
        /// importe línea factura 1.
        /// </summary>
        public decimal InvoiceLineNetAmount01 { get; set; }

        /// <summary>
        /// importe línea factura 2.
        /// </summary>
        public decimal InvoiceLineNetAmount02 { get; set; }

        /// <summary>
        /// importe línea factura 3.
        /// </summary>
        public decimal InvoiceLineNetAmount03 { get; set; }

        /// <summary>
        /// importe línea factura 4.
        /// </summary>
        public decimal InvoiceLineNetAmount04 { get; set; }

        /// <summary>
        /// importe línea factura 5.
        /// </summary>
        public decimal InvoiceLineNetAmount05 { get; set; }

        /// <summary>
        /// importe línea factura 6.
        /// </summary>
        public decimal InvoiceLineNetAmount06 { get; set; }

        /// <summary>
        /// importe línea factura 7.
        /// </summary>
        public decimal InvoiceLineNetAmount07 { get; set; }

        /// <summary>
        /// importe línea factura 8.
        /// </summary>
        public decimal InvoiceLineNetAmount08 { get; set; }

        /// <summary>
        /// importe línea factura 9.
        /// </summary>
        public decimal InvoiceLineNetAmount09 { get; set; }

        /// <summary>
        /// importe línea factura 10.
        /// </summary>
        public decimal InvoiceLineNetAmount10 { get; set; }

        #endregion

    }
}
