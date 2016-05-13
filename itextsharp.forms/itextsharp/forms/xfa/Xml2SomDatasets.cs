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
using System.Collections.Generic;
using Org.W3c.Dom;

namespace iTextSharp.Forms.Xfa
{
	/// <summary>Processes the datasets section in the XFA form.</summary>
	internal class Xml2SomDatasets : Xml2Som
	{
		/// <summary>Creates a new instance from the datasets node.</summary>
		/// <remarks>
		/// Creates a new instance from the datasets node. This expects
		/// not the datasets but the data node that comes below.
		/// </remarks>
		/// <param name="n">the datasets node</param>
		public Xml2SomDatasets(Node n)
		{
			order = new List<String>();
			name2Node = new Dictionary<String, Node>();
			stack = new Stack<String>();
			anform = 0;
			inverseSearch = new Dictionary<String, InverseStore>();
			ProcessDatasetsInternal(n);
		}

		/// <summary>Inserts a new <CODE>Node</CODE> that will match the short name.</summary>
		/// <param name="n">the datasets top <CODE>Node</CODE></param>
		/// <param name="shortName">the short name</param>
		/// <returns>the new <CODE>Node</CODE> of the inserted name</returns>
		public virtual Node InsertNode(Node n, String shortName)
		{
			Stack<String> localStack = SplitParts(shortName);
			Document doc = n.GetOwnerDocument();
			Node n2 = null;
			n = n.GetFirstChild();
			while (n.GetNodeType() != Node.ELEMENT_NODE)
			{
				n = n.GetNextSibling();
			}
			for (int k = 0; k < localStack.Count; ++k)
			{
				String part = localStack[k];
				int idx = part.LastIndexOf('[');
				String name = part.JSubstring(0, idx);
				idx = System.Convert.ToInt32(part.JSubstring(idx + 1, part.Length - 1));
				int found = -1;
				for (n2 = n.GetFirstChild(); n2 != null; n2 = n2.GetNextSibling())
				{
					if (n2.GetNodeType() == Node.ELEMENT_NODE)
					{
						String s = EscapeSom(n2.GetLocalName());
						if (s.Equals(name))
						{
							++found;
							if (found == idx)
							{
								break;
							}
						}
					}
				}
				for (; found < idx; ++found)
				{
					n2 = doc.CreateElementNS(null, name);
					n2 = n.AppendChild(n2);
					Node attr = doc.CreateAttributeNS(XfaForm.XFA_DATA_SCHEMA, "dataNode");
					attr.SetNodeValue("dataGroup");
					n2.GetAttributes().SetNamedItemNS(attr);
				}
				n = n2;
			}
			InverseSearchAdd(inverseSearch, localStack, shortName);
			name2Node[shortName] = n2;
			order.Add(shortName);
			return n2;
		}

		private static bool HasChildren(Node n)
		{
			Node dataNodeN = n.GetAttributes().GetNamedItemNS(XfaForm.XFA_DATA_SCHEMA, "dataNode"
				);
			if (dataNodeN != null)
			{
				String dataNode = dataNodeN.GetNodeValue();
				if ("dataGroup".Equals(dataNode))
				{
					return true;
				}
				else
				{
					if ("dataValue".Equals(dataNode))
					{
						return false;
					}
				}
			}
			if (!n.HasChildNodes())
			{
				return false;
			}
			Node n2 = n.GetFirstChild();
			while (n2 != null)
			{
				if (n2.GetNodeType() == Node.ELEMENT_NODE)
				{
					return true;
				}
				n2 = n2.GetNextSibling();
			}
			return false;
		}

		private void ProcessDatasetsInternal(Node n)
		{
			if (n != null)
			{
				IDictionary<String, int> ss = new Dictionary<String, int>();
				Node n2 = n.GetFirstChild();
				while (n2 != null)
				{
					if (n2.GetNodeType() == Node.ELEMENT_NODE)
					{
						String s = EscapeSom(n2.GetLocalName());
						int i = ss[s];
						if (i == null)
						{
							i = 0;
						}
						else
						{
							i += 1;
						}
						ss[s] = i;
						if (HasChildren(n2))
						{
							stack.Push(s + "[" + i.ToString() + "]");
							ProcessDatasetsInternal(n2);
							stack.Pop();
						}
						else
						{
							stack.Push(s + "[" + i.ToString() + "]");
							String unstack = PrintStack();
							order.Add(unstack);
							InverseSearchAdd(unstack);
							name2Node[unstack] = n2;
							stack.Pop();
						}
					}
					n2 = n2.GetNextSibling();
				}
			}
		}
	}
}
