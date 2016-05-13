/*
$Id$

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Collection
{
	public class PdfCollection : PdfObjectWrapper<PdfDictionary>
	{
		private const long serialVersionUID = 5184499156015360355L;

		/// <summary>A type of initial view</summary>
		public const int DETAILS = 0;

		/// <summary>A type of initial view</summary>
		public const int TILE = 1;

		/// <summary>A type of initial view</summary>
		public const int HIDDEN = 2;

		public PdfCollection(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		/// <summary>Constructs a PDF Collection.</summary>
		public PdfCollection()
			: this(new PdfDictionary())
		{
		}

		/// <summary>Sets the Collection schema dictionary.</summary>
		/// <param name="schema">an overview of the collection fields</param>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Collection.PdfCollection SetSchema(PdfCollectionSchema
			 schema)
		{
			GetPdfObject().Put(PdfName.Schema, schema.GetPdfObject());
			return this;
		}

		public virtual PdfCollectionSchema GetSchema()
		{
			return new PdfCollectionSchema(GetPdfObject().GetAsDictionary(PdfName.Schema));
		}

		/// <summary>
		/// Identifies the document that will be initially presented
		/// in the user interface.
		/// </summary>
		/// <param name="documentName">a string that identifies an entry in the EmbeddedFiles name tree
		/// 	</param>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Collection.PdfCollection SetInitialDocument(
			String documentName)
		{
			GetPdfObject().Put(PdfName.D, new PdfString(documentName));
			return this;
		}

		public virtual PdfString GetInitialDocument()
		{
			return GetPdfObject().GetAsString(PdfName.D);
		}

		/// <summary>Sets the initial view.</summary>
		/// <param name="viewType"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Collection.PdfCollection SetView(int viewType
			)
		{
			switch (viewType)
			{
				default:
				{
					GetPdfObject().Put(PdfName.View, PdfName.D);
					break;
				}

				case TILE:
				{
					GetPdfObject().Put(PdfName.View, PdfName.T);
					break;
				}

				case HIDDEN:
				{
					GetPdfObject().Put(PdfName.View, PdfName.H);
					break;
				}
			}
			return this;
		}

		public virtual PdfNumber GetView()
		{
			return GetPdfObject().GetAsNumber(PdfName.View);
		}

		/// <summary>Sets the Collection sort dictionary.</summary>
		/// <param name="sort"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Collection.PdfCollection SetSort(PdfCollectionSort
			 sort)
		{
			GetPdfObject().Put(PdfName.Sort, sort.GetPdfObject());
			return this;
		}

		public virtual PdfCollectionSort GetSort()
		{
			return new PdfCollectionSort(GetPdfObject().GetAsDictionary(PdfName.Sort));
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return false;
		}
	}
}
