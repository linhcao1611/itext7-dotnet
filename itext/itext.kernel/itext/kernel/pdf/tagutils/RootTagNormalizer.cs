using System;
using System.Collections.Generic;
using Common.Logging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;

namespace iText.Kernel.Pdf.Tagutils {
    internal class RootTagNormalizer {
        private TagStructureContext context;

        private PdfStructElem rootTagElement;

        private PdfDocument document;

        internal RootTagNormalizer(TagStructureContext context, PdfStructElem rootTagElement, PdfDocument document
            ) {
            this.context = context;
            this.rootTagElement = rootTagElement;
            this.document = document;
        }

        internal virtual PdfStructElem MakeSingleStandardRootTag(IList<IStructureNode> rootKids) {
            document.GetStructTreeRoot().MakeIndirect(document);
            if (rootTagElement == null) {
                CreateNewRootTag();
            }
            else {
                rootTagElement.MakeIndirect(document);
                document.GetStructTreeRoot().AddKid(rootTagElement);
                EnsureExistingRootTagIsDocument();
            }
            AddStructTreeRootKidsToTheRootTag(rootKids);
            return rootTagElement;
        }

        private void CreateNewRootTag() {
            IRoleMappingResolver mapping;
            PdfNamespace docDefaultNs = context.GetDocumentDefaultNamespace();
            mapping = context.ResolveMappingToStandardOrDomainSpecificRole(StandardRoles.DOCUMENT, docDefaultNs);
            if (mapping == null || mapping.CurrentRoleIsStandard() && !StandardRoles.DOCUMENT.Equals(mapping.GetRole()
                )) {
                LogCreatedRootTagHasMappingIssue(docDefaultNs, mapping);
            }
            rootTagElement = document.GetStructTreeRoot().AddKid(new PdfStructElem(document, PdfName.Document));
            if (context.TargetTagStructureVersionIs2()) {
                rootTagElement.SetNamespace(docDefaultNs);
                context.EnsureNamespaceRegistered(docDefaultNs);
            }
        }

        private void EnsureExistingRootTagIsDocument() {
            IRoleMappingResolver mapping;
            mapping = context.GetRoleMappingResolver(rootTagElement.GetRole().GetValue(), rootTagElement.GetNamespace(
                ));
            bool isDocBeforeResolving = mapping.CurrentRoleIsStandard() && StandardRoles.DOCUMENT.Equals(mapping.GetRole
                ());
            mapping = context.ResolveMappingToStandardOrDomainSpecificRole(rootTagElement.GetRole().GetValue(), rootTagElement
                .GetNamespace());
            bool isDocAfterResolving = mapping != null && mapping.CurrentRoleIsStandard() && StandardRoles.DOCUMENT.Equals
                (mapping.GetRole());
            if (isDocBeforeResolving && !isDocAfterResolving) {
                LogCreatedRootTagHasMappingIssue(rootTagElement.GetNamespace(), mapping);
            }
            else {
                if (!isDocAfterResolving) {
                    WrapAllKidsInTag(rootTagElement, rootTagElement.GetRole(), rootTagElement.GetNamespace());
                    rootTagElement.SetRole(PdfName.Document);
                    if (context.TargetTagStructureVersionIs2()) {
                        rootTagElement.SetNamespace(context.GetDocumentDefaultNamespace());
                        context.EnsureNamespaceRegistered(context.GetDocumentDefaultNamespace());
                    }
                }
            }
        }

        private void AddStructTreeRootKidsToTheRootTag(IList<IStructureNode> rootKids) {
            int originalRootKidsIndex = 0;
            bool isBeforeOriginalRoot = true;
            foreach (IStructureNode elem in rootKids) {
                // StructTreeRoot kids are always PdfStructElement, so we are save here to cast it
                PdfStructElem kid = (PdfStructElem)elem;
                if (kid.GetPdfObject() == rootTagElement.GetPdfObject()) {
                    isBeforeOriginalRoot = false;
                    continue;
                }
                // This boolean is used to "flatten" possible deep "stacking" of the tag structure in case of the multiple pages copying operations.
                // This could happen due to the wrapping of all the kids in the createNewRootTag or ensureExistingRootTagIsDocument methods.
                // And therefore, we don't need here to resolve mappings, because we exactly know which role we set.
                bool kidIsDocument = PdfName.Document.Equals(kid.GetRole());
                if (kidIsDocument && kid.GetNamespace() != null && context.TargetTagStructureVersionIs2()) {
                    // we flatten only tags of document role in standard structure namespace
                    String kidNamespaceName = kid.GetNamespace().GetNamespaceName();
                    kidIsDocument = StandardNamespaces.PDF_1_7.Equals(kidNamespaceName) || StandardNamespaces.PDF_2_0.Equals(kidNamespaceName
                        );
                }
                if (isBeforeOriginalRoot) {
                    rootTagElement.AddKid(originalRootKidsIndex, kid);
                    originalRootKidsIndex += kidIsDocument ? kid.GetKids().Count : 1;
                }
                else {
                    rootTagElement.AddKid(kid);
                }
                if (kidIsDocument) {
                    RemoveOldRoot(kid);
                }
            }
        }

        private void WrapAllKidsInTag(PdfStructElem parent, PdfName wrapTagRole, PdfNamespace wrapTagNs) {
            int kidsNum = parent.GetKids().Count;
            TagTreePointer tagPointer = new TagTreePointer(parent, document);
            tagPointer.AddTag(0, wrapTagRole.GetValue());
            if (context.TargetTagStructureVersionIs2()) {
                tagPointer.GetProperties().SetNamespace(wrapTagNs);
            }
            TagTreePointer newParentOfKids = new TagTreePointer(tagPointer);
            tagPointer.MoveToParent();
            for (int i = 0; i < kidsNum; ++i) {
                tagPointer.RelocateKid(1, newParentOfKids);
            }
        }

        private void RemoveOldRoot(PdfStructElem oldRoot) {
            TagTreePointer tagPointer = new TagTreePointer(document);
            tagPointer.SetCurrentStructElem(oldRoot).RemoveTag();
        }

        private void LogCreatedRootTagHasMappingIssue(PdfNamespace rootTagOriginalNs, IRoleMappingResolver mapping
            ) {
            String origRootTagNs = "";
            if (rootTagOriginalNs != null && rootTagOriginalNs.GetNamespaceName() != null) {
                origRootTagNs = " in \"" + rootTagOriginalNs.GetNamespaceName() + "\" namespace";
            }
            String mappingRole = " to ";
            if (mapping != null) {
                mappingRole += "\"" + mapping.GetRole() + "\"";
                if (mapping.GetNamespace() != null && !StandardNamespaces.PDF_1_7.Equals(mapping.GetNamespace().GetNamespaceName
                    ())) {
                    mappingRole += " in \"" + mapping.GetNamespace().GetNamespaceName() + "\" namespace";
                }
            }
            else {
                mappingRole += "not standard role";
            }
            ILog logger = LogManager.GetLogger(typeof(iText.Kernel.Pdf.Tagutils.RootTagNormalizer));
            logger.Warn(String.Format(iText.IO.LogMessageConstant.CREATED_ROOT_TAG_HAS_MAPPING, origRootTagNs, mappingRole
                ));
        }
    }
}