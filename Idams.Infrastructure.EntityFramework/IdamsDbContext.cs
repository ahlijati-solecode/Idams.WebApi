using Idams.Core.Model.Entities;
using Idams.Infrastructure.EntityFramework.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Idams.Infrastructure.EntityFramework
{
    public partial class IdamsDbContext : DbContext
    {
        public IdamsDbContext(DbContextOptions options) : base(options)
        {

        }

        public virtual DbSet<LgProjectActivityAuditTrail> LgProjectActivityAuditTrails { get; set; } = null!;
        public virtual DbSet<LgProjectHeaderAuditTrail> LgProjectHeaderAuditTrails { get; set; } = null!;
        public virtual DbSet<MdDocumentDescription> MdDocumentDescriptions { get; set; } = null!;
        public virtual DbSet<MdDocumentGroup> MdDocumentGroups { get; set; } = null!;
        public virtual DbSet<MdFidcode> MdFidcodes { get; set; } = null!;
        public virtual DbSet<MdParamater> MdParamaters { get; set; } = null!;
        public virtual DbSet<MdParamaterList> MdParamaterLists { get; set; } = null!;
        public virtual DbSet<MpDocumentChecklist> MpDocumentChecklists { get; set; } = null!;
        public virtual DbSet<RefTemplate> RefTemplates { get; set; } = null!;
        public virtual DbSet<RefTemplateDocument> RefTemplateDocuments { get; set; } = null!;
        public virtual DbSet<RefTemplateWorkflowSequence> RefTemplateWorkflowSequences { get; set; } = null!;
        public virtual DbSet<RefThresholdProject> RefThresholdProjects { get; set; } = null!;
        public virtual DbSet<RefWorkflow> RefWorkflows { get; set; } = null!;
        public virtual DbSet<RefWorkflowAction> RefWorkflowActions { get; set; } = null!;
        public virtual DbSet<RefWorkflowActor> RefWorkflowActors { get; set; } = null!;
        public virtual DbSet<TxApproval> TxApprovals { get; set; } = null!;
        public virtual DbSet<TxDocument> TxDocuments { get; set; } = null!;
        public virtual DbSet<TxFollowUp> TxFollowUps { get; set; } = null!;
        public virtual DbSet<TxMeeting> TxMeetings { get; set; } = null!;
        public virtual DbSet<TxMeetingParticipant> TxMeetingParticipants { get; set; } = null!;
        public virtual DbSet<TxProjectAction> TxProjectActions { get; set; } = null!;
        public virtual DbSet<TxProjectComment> TxProjectComments { get; set; } = null!;
        public virtual DbSet<TxProjectCompressor> TxProjectCompressors { get; set; } = null!;
        public virtual DbSet<TxProjectEquipment> TxProjectEquipments { get; set; } = null!;
        public virtual DbSet<TxProjectHeader> TxProjectHeaders { get; set; } = null!;
        public virtual DbSet<TxProjectListColumnConfig> TxProjectListColumnConfigs { get; set; } = null!;
        public virtual DbSet<TxProjectMilestone> TxProjectMilestones { get; set; } = null!;
        public virtual DbSet<TxProjectPipeline> TxProjectPipelines { get; set; } = null!;
        public virtual DbSet<TxProjectPlatform> TxProjectPlatforms { get; set; } = null!;
        public virtual DbSet<TxProjectUpstreamEntity> TxProjectUpstreamEntities { get; set; } = null!;
        public virtual DbSet<VwDistinctHierLvlType> VwDistinctHierLvlTypes { get; set; } = null!;
        public virtual DbSet<VwShuhier03> VwShuhier03s { get; set; } = null!;
        public virtual DbSet<VwUserHierLvl> VwUserHierLvls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LgProjectActivityAuditTrail>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.CreatedDate })
                    .HasName("PK__LG_Proje__3E70E81776BE2E4F");

                entity.ToTable("LG_ProjectActivityAuditTrail", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Action)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ActivityStatusParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmpAccount)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.WorkflowSequence)
                    .WithMany(p => p.LgProjectActivityAuditTrails)
                    .HasForeignKey(d => d.WorkflowSequenceId)
                    .HasConstraintName("FK__LG_Projec__Workf__69FBBC1F");
            });

            modelBuilder.Entity<LgProjectHeaderAuditTrail>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ProjectVersion, e.Action })
                    .HasName("PK__LG_Proje__A110D3D17BBA227F");

                entity.ToTable("LG_ProjectHeaderAuditTrail", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Action)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Section)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.LgProjectHeaderAuditTrails)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LG_ProjectHeader__55009F39");
            });

            modelBuilder.Entity<MdDocumentDescription>(entity =>
            {
                entity.HasKey(e => e.DocDescriptionId)
                    .HasName("PK__MD_Docum__3EF188ADC0626CEA");

                entity.ToTable("MD_DocumentDescription", "idams");

                entity.Property(e => e.DocDescriptionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocCategoryParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DocDescription)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DocTypeParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MdDocumentGroup>(entity =>
            {
                entity.HasKey(e => new { e.DocGroupParId, e.DocGroupVersion })
                    .HasName("PK__MD_Docum__83C24233D24C11DF");

                entity.ToTable("MD_DocumentGroup", "idams");

                entity.Property(e => e.DocGroupParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MdFidcode>(entity =>
            {
                entity.HasKey(e => new { e.SubholdingCode, e.ProjectCategory, e.ApprovedYear, e.Regional })
                    .HasName("MD_FIDCode_PK");

                entity.ToTable("MD_FIDCode", "idams");

                entity.Property(e => e.SubholdingCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectCategory)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MdParamater>(entity =>
            {
                entity.HasKey(e => new { e.Schema, e.ParamId });

                entity.ToTable("MD_Paramater");

                entity.Property(e => e.Schema)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParamId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ParamID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamDesc)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<MdParamaterList>(entity =>
            {
                entity.HasKey(e => new { e.Schema, e.ParamId, e.ParamListId });

                entity.ToTable("MD_ParamaterList");

                entity.Property(e => e.Schema)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParamId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ParamID");

                entity.Property(e => e.ParamListId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ParamListID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ParamListDesc)
                    .HasMaxLength(400)
                    .IsUnicode(false);

                entity.Property(e => e.ParamValue1).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.ParamValue1Text)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ParamValue2).HasColumnType("decimal(8, 2)");

                entity.Property(e => e.ParamValue2Text)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.MdParamater)
                    .WithMany(p => p.MdParamaterLists)
                    .HasForeignKey(d => new { d.Schema, d.ParamId })
                    .HasConstraintName("FK_MD_ParamaterList_MD_Paramater");
            });

            modelBuilder.Entity<MpDocumentChecklist>(entity =>
            {
                entity.HasKey(e => new { e.DocGroupParId, e.DocGroupVersion, e.DocDescriptionId })
                    .HasName("PK__MP_Docum__2FFCB3BB50AC8E54");

                entity.ToTable("MP_DocumentChecklist", "idams");

                entity.Property(e => e.DocGroupParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DocDescriptionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.DocDescription)
                    .WithMany(p => p.MpDocumentChecklists)
                    .HasForeignKey(d => d.DocDescriptionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MP_Docume__DocId__7E37BEF6");

                entity.HasOne(d => d.DocGroup)
                    .WithMany(p => p.MpDocumentChecklists)
                    .HasForeignKey(d => new { d.DocGroupParId, d.DocGroupVersion })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__MP_DocumentCheck__7C4F7684");
            });

            modelBuilder.Entity<RefTemplate>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.TemplateVersion })
                    .HasName("PK__REF_Temp__3B2CEED878EBF46C");

                entity.ToTable("REF_Template", "idams");

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EndWorkflow)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectCategoryParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectCriteriaParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectSubCriteriaParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.StartWorkflow)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ThresholdNameParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.RefTemplates)
                    .HasForeignKey(d => new { d.ThresholdNameParId, d.ThresholdVersion })
                    .HasConstraintName("FK__REF_Template__76969D2E");
            });

            modelBuilder.Entity<RefTemplateDocument>(entity =>
            {
                entity.HasKey(e => new { e.TemplateId, e.TemplateVersion, e.ThresholdNameParId, e.ThresholdVersion, e.WorkflowSequenceId, e.WorkflowActionId })
                    .HasName("PK__REF_Temp__C8620BBB1D24C098");

                entity.ToTable("REF_TemplateDocument", "idams");

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ThresholdNameParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DocGroupParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.WorkflowAction)
                    .WithMany(p => p.RefTemplateDocuments)
                    .HasForeignKey(d => d.WorkflowActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REF_Templ__Workf__797309D9");

                entity.HasOne(d => d.WorkflowSequence)
                    .WithMany(p => p.RefTemplateDocuments)
                    .HasForeignKey(d => d.WorkflowSequenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REF_Templ__Workf__787EE5A0");

                entity.HasOne(d => d.DocGroup)
                    .WithMany(p => p.RefTemplateDocuments)
                    .HasForeignKey(d => new { d.DocGroupParId, d.DocGroupVersion })
                    .HasConstraintName("FK__REF_TemplateDocu__7D439ABD");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.RefTemplateDocuments)
                    .HasForeignKey(d => new { d.TemplateId, d.TemplateVersion })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REF_TemplateDocu__7A672E12");

                entity.HasOne(d => d.Threshold)
                    .WithMany(p => p.RefTemplateDocuments)
                    .HasForeignKey(d => new { d.ThresholdNameParId, d.ThresholdVersion })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REF_TemplateDocu__7B5B524B");
            });

            modelBuilder.Entity<RefTemplateWorkflowSequence>(entity =>
            {
                entity.HasKey(e => e.WorkflowSequenceId)
                    .HasName("PK__REF_Temp__D45BCAB95705BD45");

                entity.ToTable("REF_TemplateWorkflowSequence", "idams");

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Order).HasColumnName("order");

                entity.Property(e => e.Sla).HasColumnName("SLA");

                entity.Property(e => e.SlauoM)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLAUoM");

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkflowId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.RefTemplateWorkflowSequences)
                    .HasForeignKey(d => d.WorkflowId)
                    .HasConstraintName("FK__REF_Templ__Workf__10566F31");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.RefTemplateWorkflowSequences)
                    .HasForeignKey(d => new { d.TemplateId, d.TemplateVersion })
                    .HasConstraintName("FK__REF_TemplateWork__778AC167");
            });

            modelBuilder.Entity<RefThresholdProject>(entity =>
            {
                entity.HasKey(e => new { e.ThresholdNameParId, e.ThresholdVersion })
                    .HasName("PK__REF_Thre__14E48ABE177679E1");

                entity.ToTable("REF_ThresholdProject", "idams");

                entity.Property(e => e.ThresholdNameParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Capex1).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Capex2).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MathOperatorParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RefWorkflow>(entity =>
            {
                entity.HasKey(e => e.WorkflowId)
                    .HasName("PK__REF_Work__5704A66A347D010F");

                entity.ToTable("REF_Workflow", "idams");

                entity.Property(e => e.WorkflowId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowCategoryParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowMadamPk)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("WorkflowMadamPK");

                entity.Property(e => e.WorkflowType)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RefWorkflowAction>(entity =>
            {
                entity.HasKey(e => e.WorkflowActionId)
                    .HasName("PK__REF_Work__5B286E2C959CAEAA");

                entity.ToTable("REF_WorkflowAction", "idams");

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.RequiredDocumentGroupParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowActionName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowActionTypeParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.RefWorkflowActions)
                    .HasForeignKey(d => d.WorkflowId)
                    .HasConstraintName("FK__REF_Workf__Workf__75A278F5");
            });

            modelBuilder.Entity<RefWorkflowActor>(entity =>
            {
                entity.HasKey(e => new { e.WorkflowActionId, e.Action, e.Actor })
                    .HasName("PK__REF_Work__1A0CD2553D246202");

                entity.ToTable("REF_WorkflowActor", "idams");

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Action)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Actor)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.WorkflowAction)
                    .WithMany(p => p.RefWorkflowActors)
                    .HasForeignKey(d => d.WorkflowActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__REF_Workf__Workf__662B2B3B");
            });

            modelBuilder.Entity<TxApproval>(entity =>
            {
                entity.HasKey(e => new { e.ApprovalId, e.ProjectActionId })
                    .HasName("PK__TX_Appro__D0BE76C5CA979516");

                entity.ToTable("TX_Approval", "idams");

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovalBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ApprovalDate).HasColumnType("datetime");

                entity.Property(e => e.ApprovalStatusParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProjectAction)
                    .WithMany(p => p.TxApprovals)
                    .HasForeignKey(d => d.ProjectActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_Approv__Proje__7D0E9093");
            });

            modelBuilder.Entity<TxDocument>(entity =>
            {
                entity.HasKey(e => e.TransactionDocId)
                    .HasName("PK__TX_Docum__60F73D49A5A46392");

                entity.ToTable("TX_Document", "idams");

                entity.Property(e => e.TransactionDocId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.DocDescriptionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DocName)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.FileExtension)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FilePath)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.FileSizeUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.LastUpdateWorkflowSequence)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ThresholdNameParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.DocDescription)
                    .WithMany(p => p.TxDocuments)
                    .HasForeignKey(d => d.DocDescriptionId)
                    .HasConstraintName("FK__TX_Docume__DocDe__59C55456");

                entity.HasOne(d => d.ProjectAction)
                    .WithMany(p => p.TxDocuments)
                    .HasForeignKey(d => d.ProjectActionId)
                    .HasConstraintName("FK__TX_Docume__Proje__58D1301D");

                entity.HasOne(d => d.RefTemplateDocument)
                    .WithMany(p => p.TxDocuments)
                    .HasForeignKey(d => new { d.TemplateId, d.TemplateVersion, d.ThresholdNameParId, d.ThresholdVersion, d.WorkflowSequenceId, d.WorkflowActionId })
                    .HasConstraintName("FK__TX_Document__57DD0BE4");
            });

            modelBuilder.Entity<TxFollowUp>(entity =>
            {
                entity.HasKey(e => new { e.FollowUpId, e.ProjectActionId })
                    .HasName("PK__TX_Follo__968678FF5F15FCFC");

                entity.ToTable("TX_FollowUp", "idams");

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FollowUpAspectParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.PositionFunction)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Recommendation)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ReviewResult)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.ReviewerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RiskDescription)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.RiskLevelParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ProjectAction)
                    .WithMany(p => p.TxFollowUps)
                    .HasForeignKey(d => d.ProjectActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_Follow__Proje__671F4F74");
            });

            modelBuilder.Entity<TxMeeting>(entity =>
            {
                entity.HasKey(e => new { e.MeetingId, e.ProjectActionId })
                    .HasName("PK__TX_Meeti__0BC3E87D7495B94A");

                entity.ToTable("TX_Meeting", "idams");

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Location)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.MeetingStatusParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(8000)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProjectAction)
                    .WithMany(p => p.TxMeetings)
                    .HasForeignKey(d => d.ProjectActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_Meetin__Proje__793DFFAF");
            });

            modelBuilder.Entity<TxMeetingParticipant>(entity =>
            {
                entity.HasKey(e => new { e.MeetingId, e.ProjectActionId, e.EmpEmail })
                    .HasName("PK__TX_Meeti__DDB70CDE8995992E");

                entity.ToTable("TX_MeetingParticipants", "idams");

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmpEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpAccount)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.TxMeeting)
                    .WithMany(p => p.TxMeetingParticipants)
                    .HasForeignKey(d => new { d.MeetingId, d.ProjectActionId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_MeetingPartic__078C1F06");
            });

            modelBuilder.Entity<TxProjectAction>(entity =>
            {
                entity.HasKey(e => e.ProjectActionId)
                    .HasName("PK__TX_Proje__23A0131F3ED9A996");

                entity.ToTable("TX_ProjectAction", "idams");

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.AimanTransactionNumber)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WorkflowActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.WorkflowAction)
                    .WithMany(p => p.TxProjectActions)
                    .HasForeignKey(d => d.WorkflowActionId)
                    .HasConstraintName("FK__TX_Projec__Workf__4B7734FF");

                entity.HasOne(d => d.WorkflowSequence)
                    .WithMany(p => p.TxProjectActions)
                    .HasForeignKey(d => d.WorkflowSequenceId)
                    .HasConstraintName("FK__TX_Projec__Workf__4C6B5938");
            });

            modelBuilder.Entity<TxProjectComment>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ProjectCommentId })
                    .HasName("PK__TX_Proje__074BCBFCB8E648A8");

                entity.ToTable("TX_ProjectComment", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Comment)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectActionId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProjectAction)
                    .WithMany(p => p.TxProjectComments)
                    .HasForeignKey(d => d.ProjectActionId)
                    .HasConstraintName("FK__TX_Projec__Proje__02C769E9");
            });

            modelBuilder.Entity<TxProjectCompressor>(entity =>
            {
                entity.HasKey(e => e.ProjectCompressorId)
                    .HasName("PK__TX_Proje__AABD69AC9A64FF28");

                entity.ToTable("TX_ProjectCompressor", "idams");

                entity.Property(e => e.ProjectCompressorId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CompressorCapacity).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CompressorCapacityUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CompressorDischargePressure).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.CompressorDischargePressureUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CompressorTypeParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.TxProjectCompressors)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .HasConstraintName("FK__TX_ProjectCompre__498EEC8D");
            });

            modelBuilder.Entity<TxProjectEquipment>(entity =>
            {
                entity.HasKey(e => e.ProjectEquipmentId)
                    .HasName("PK__TX_Proje__E6081CA2821ACA73");

                entity.ToTable("TX_ProjectEquipment", "idams");

                entity.Property(e => e.ProjectEquipmentId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EquipmentName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.TxProjectEquipments)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .HasConstraintName("FK__TX_ProjectEquipm__4A8310C6");
            });

            modelBuilder.Entity<TxProjectHeader>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ProjectVersion })
                    .HasName("PK__TX_Proje__ED78691970154552");

                entity.ToTable("TX_ProjectHeader", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.BenefitCostRatio).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.Capex).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.CapexUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CurrentAction)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentWorkflowSequence)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.DiscountedPayOutTime).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.DrillingCost).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.DrillingCostUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.EstFidapproved)
                    .HasColumnType("date")
                    .HasColumnName("EstFIDApproved");

                entity.Property(e => e.FacilitiesCost).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.FacilitiesCostUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Fidcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FIDCode");

                entity.Property(e => e.Gas).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.GasUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.InitiationAction)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.InitiationDate).HasColumnType("datetime");

                entity.Property(e => e.InternalRateOfReturn).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.LastUpdateWorkflowSequence)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.NetPresentValue).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Oil).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.OilEquivalent).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.OilEquivalentUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OilUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ParticipatingInterest).HasColumnType("decimal(5, 2)");

                entity.Property(e => e.ProfitabilityIndex).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectOnStream).HasColumnType("date");

                entity.Property(e => e.ProposalDate).HasColumnType("date");

                entity.Property(e => e.Pvin)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("PVIn");

                entity.Property(e => e.Pvout)
                    .HasColumnType("decimal(12, 2)")
                    .HasColumnName("PVOut");

                entity.Property(e => e.Rkap).HasColumnName("RKAP");

                entity.Property(e => e.Status)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.TxProjectHeaders)
                    .HasForeignKey(d => new { d.TemplateId, d.TemplateVersion })
                    .HasConstraintName("FK__TX_ProjectHeader__4D5F7D71");
            });

            modelBuilder.Entity<TxProjectListColumnConfig>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__TX_UserT__1788CC4CF18E9604");

                entity.ToTable("TX_ProjectListColumnConfig", "idams");

                entity.Property(e => e.UserId)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.Config)
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TxProjectMilestone>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ProjectVersion, e.WorkflowSequenceId })
                    .HasName("PK__TX_Proje__55AC32D391F7E211");

                entity.ToTable("TX_ProjectMilestone", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.WorkflowSequenceId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.WorkflowSequence)
                    .WithMany(p => p.TxProjectMilestones)
                    .HasForeignKey(d => d.WorkflowSequenceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_Projec__Workf__45BE5BA9");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.TxProjectMilestones)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TX_ProjectMilest__46B27FE2");
            });

            modelBuilder.Entity<TxProjectPipeline>(entity =>
            {
                entity.HasKey(e => e.ProjectPipelineId)
                    .HasName("PK__TX_Proje__A16C10F7BF12644A");

                entity.ToTable("TX_ProjectPipeline", "idams");

                entity.Property(e => e.ProjectPipelineId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.FieldServiceParId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PipelineLenght).HasColumnType("decimal(9, 2)");

                entity.Property(e => e.PipelineLenghtUoM)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.TxProjectPipelines)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .HasConstraintName("FK__TX_ProjectPipeli__489AC854");
            });

            modelBuilder.Entity<TxProjectPlatform>(entity =>
            {
                entity.HasKey(e => e.ProjectPlateformId)
                    .HasName("PK__TX_Proje__15485A0F1B81D929");

                entity.ToTable("TX_ProjectPlatform", "idams");

                entity.Property(e => e.ProjectPlateformId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.TxProjectPlatforms)
                    .HasForeignKey(d => new { d.ProjectId, d.ProjectVersion })
                    .HasConstraintName("FK__TX_ProjectPlatfo__47A6A41B");
            });

            modelBuilder.Entity<TxProjectUpstreamEntity>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.EntityId })
                    .HasName("PK__TX_Proje__BFD22C093C3CA882");

                entity.ToTable("TX_ProjectUpstreamEntity", "idams");

                entity.Property(e => e.ProjectId)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EntityId)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VwDistinctHierLvlType>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_DistinctHierLvlType", "idams");
            });

            modelBuilder.Entity<VwShuhier03>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_SHUHier03", "idams");

                entity.Property(e => e.CompanyCodeDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.EntityId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EntityID");

                entity.Property(e => e.HierLvl2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HierLvl2Desc)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.HierLvl3)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HierLvl3Desc)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.HierLvl4)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HierLvl4Desc)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VwUserHierLvl>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("vw_UserHierLvl", "idams");

                entity.Property(e => e.Lvl1EntityId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Lvl1EntityID");

                entity.Property(e => e.Lvl1EntityName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Lvl2EntityId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Lvl2EntityID");

                entity.Property(e => e.Lvl2EntityName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Lvl3EntityId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Lvl3EntityID");

                entity.Property(e => e.Lvl3EntityName)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OrgUnitId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("OrgUnitID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
