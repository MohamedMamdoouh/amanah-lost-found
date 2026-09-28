namespace Amanah.Contracts.Errors;

public record ApiError(
    string Code,
    string Message,
    Dictionary<string, string[]>? Errors = null);

public static class ErrorCodes
{
    public const string ValidationFailed = "validation.failed";
    public const string InvalidPhone = "auth.invalid_phone";
    public const string InvalidEmail = "auth.invalid_email";
    public const string EmailUnavailable = "service.email_unavailable";
    public const string CaptchaFailed = "auth.captcha_failed";
    public const string InvalidOtp = "auth.invalid_otp";
    public const string OtpExpired = "auth.otp_expired";
    public const string OtpVoid = "auth.otp_void";
    public const string HandoffTokenInvalid = "auth.handoff_token_invalid";
    public const string InvalidCredentials = "auth.invalid_credentials";
    public const string AccountExists = "auth.account_exists";
    public const string TokenExpired = "auth.token_expired";
    public const string RefreshInvalid = "auth.refresh_invalid";
    public const string Banned = "auth.banned";
    public const string Unauthorized = "auth.unauthorized";
    public const string Forbidden = "auth.forbidden";
    public const string AdminParticipationForbidden = "auth.admin_participation_forbidden";
    public const string NotFound = "resource.not_found";
    public const string Unavailable = "resource.unavailable";
    public const string NotImplemented = "resource.not_implemented";
    public const string Conflict = "resource.conflict";
    public const string OtpCooldown = "otp.cooldown";
    public const string OtpHourlyLimit = "otp.hourly_limit";
    public const string OtpDailyLimit = "otp.daily_limit";
    public const string RateLimitExceeded = "rate_limit.exceeded";
    public const string SmsUnavailable = "service.sms_unavailable";
    public const string InternalError = "internal.error";

    public const string FieldPhoneRequired = "field.phone.required";
    public const string FieldPhoneInvalid = "field.phone.invalid";
    public const string FieldIdentifierRequired = "field.identifier.required";
    public const string FieldIdentifierInvalid = "field.identifier.invalid";
    public const string FieldEmailInvalid = "field.email.invalid";
    public const string FieldDisplayNameRequired = "field.display_name.required";
    public const string FieldDisplayNameInvalid = "field.display_name.invalid";
    public const string FieldAcceptTermsRequired = "field.accept_terms.required";
    public const string FieldSignupTokenRequired = "field.signup_token.required";
    public const string FieldCaptchaTokenRequired = "field.captcha_token.required";
    public const string FieldPasswordRequired = "field.password.required";
    public const string FieldPasswordTooShort = "field.password.too_short";
    public const string FieldPasswordInvalid = "field.password.invalid";
    public const string FieldResetTokenRequired = "field.reset_token.required";
    public const string FieldOtpPurposeRequired = "field.otp_purpose.required";
    public const string FieldOtpPurposeInvalid = "field.otp_purpose.invalid";
    public const string FieldRefreshTokenRequired = "field.refresh_token.required";
    public const string FieldOtpCodeRequired = "field.otp_code.required";
    public const string FieldOtpCodeInvalid = "field.otp_code.invalid";
    public const string FieldReplyEmailRequired = "field.reply_email.required";
    public const string FieldReplyEmailInvalid = "field.reply_email.invalid";
    public const string FieldSupportMessageRequired = "field.support_message.required";
    public const string FieldSupportMessageInvalid = "field.support_message.invalid";
    public const string FieldSignInChannelRequired = "field.sign_in_channel.required";
    public const string FieldSignInChannelInvalid = "field.sign_in_channel.invalid";
    public const string FieldEmailRequired = "field.email.required";
    public const string FieldSubmittedAnswerTooShort = "field.submitted_answer.too_short";
    public const string FieldSubmittedAnswerTooLong = "field.submitted_answer.too_long";
    public const string FieldPhotoMaxCount = "field.photo.max_count";
    public const string FieldPhotoEmpty = "field.photo.empty";
    public const string FieldPhotosMaxCount = "field.photos.max_count";
    public const string FieldClaimMultipartRequired = "field.claim.multipart_required";
    public const string FieldClaimDataRequired = "field.claim.data_required";
    public const string FieldClaimDataInvalid = "field.claim.data_invalid";

    public const string FieldReportTypeRequired = "field.report.type.required";
    public const string FieldReportTypeInvalid = "field.report.type.invalid";
    public const string FieldReportCategoryRequired = "field.report.category.required";
    public const string FieldReportGovernorateRequired = "field.report.governorate.required";
    public const string FieldReportTitleRequired = "field.report.title.required";
    public const string FieldReportTitleTooShort = "field.report.title.too_short";
    public const string FieldReportTitleTooLong = "field.report.title.too_long";
    public const string FieldReportDescriptionRequired = "field.report.description.required";
    public const string FieldReportDescriptionTooShort = "field.report.description.too_short";
    public const string FieldReportDescriptionTooLong = "field.report.description.too_long";
    public const string FieldReportAreaTooLong = "field.report.area_text.too_long";
    public const string FieldReportDateFuture = "field.report.date.future";
    public const string FieldReportDateTooOld = "field.report.date.too_old";
    public const string FieldReportRewardFoundOnly = "field.report.reward.found_only";
    public const string FieldReportRewardRequired = "field.report.reward.required";
    public const string FieldReportRewardRange = "field.report.reward.range";
    public const string FieldReportRewardEmpty = "field.report.reward.empty";
    public const string FieldReportHeldLocationLostOnly = "field.report.held_location.lost_only";
    public const string FieldReportHeldLocationRequired = "field.report.held_location.required";
    public const string FieldReportHeldLocationTooLong = "field.report.held_location.too_long";
    public const string FieldCategoryUnknown = "field.category.unknown";
    public const string FieldCategoryRequired = "field.category.required";
    public const string FieldCategoryMinLength = "field.category.min_length";
    public const string FieldCategoryMaxLength = "field.category.max_length";
    public const string FieldCategoryLettersAndSpaces = "field.category.letters_and_spaces";

    public const string ClaimContactInfo = "claim.contact_info";

    public const string FieldAbuseReasonRequired = "field.abuse.reason.required";
    public const string FieldAbuseNoteTooLong = "field.abuse.note.too_long";
    public const string FieldReportWithdrawReasonInvalid = "field.report.withdraw_reason.invalid";
    public const string FieldAdminRejectionReasonRequired = "field.admin.rejection_reason.required";
    public const string FieldAdminRejectionReasonInvalid = "field.admin.rejection_reason.invalid";
    public const string FieldAdminRejectionNoteTooLong = "field.admin.rejection_note.too_long";

    public const string ReportDailyQuota = "report.daily_quota";
    public const string ReportOpenCap = "report.open_cap";
    public const string ReportContactInfo = "report.contact_info";
    public const string ReportResubmitCap = "report.resubmit_cap";

    public const string ClaimDailyQuota = "claim.daily_quota";
    public const string ClaimAttemptLimit = "claim.attempt_limit";
    public const string ClaimPendingExists = "claim.pending_exists";
    public const string ClaimOwnReport = "claim.own_report";
    public const string ClaimInvalidStatus = "claim.invalid_status";

    public const string UploadInvalidFormat = "upload.invalid_format";
    public const string UploadTooLarge = "upload.too_large";
    public const string UploadStorageFailed = "upload.storage_failed";

    public const string ChatReadOnly = "chat.read_only";

    public const string AccountDeactivated = "account.deactivated";
    public const string AccountDeactivationBlocked = "account.deactivation_blocked";
    public const string AccountDeactivationAlreadyRequested = "account.deactivation_already_requested";
    public const string AccountReactivationRequired = "account.reactivation_required";
    public const string AccountBlockerClaimInProgress = "claim_in_progress";
    public const string AccountBlockerApprovedClaim = "approved_claim";
    public const string AccountBlockerAdminRole = "admin_role";

    public const string AbuseDuplicateFlag = "abuse.duplicate_flag";
    public const string AbuseInvalidReason = "abuse.invalid_reason";
    public const string AbuseCannotFlagOwnListing = "abuse.cannot_flag_own_listing";
    public const string AbuseListingNotFlaggable = "abuse.listing_not_flaggable";
    public const string AbuseAlreadyResolved = "abuse.already_resolved";
    public const string AbuseNotOpen = "abuse.not_open";
    public const string AbuseInvalidOutcome = "abuse.invalid_outcome";
    public const string AbuseInvestigationUnavailable = "abuse.investigation_unavailable";

    public const string EnforcementUserAlreadyBanned = "enforcement.user_already_banned";
    public const string EnforcementUserNotBanned = "enforcement.user_not_banned";
    public const string EnforcementReportNotTakedownable = "enforcement.report_not_takedownable";
}
