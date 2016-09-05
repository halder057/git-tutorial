using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiNotification.Utilities
{
    public class Constants
    {
        public const string DeliveryRecipientType_Contact = "contact";
        public const string InvalidCredential = "Invalid username or password";
        public const string InsufficientPrivilege = "Insufficient Privilege";

        public const string FeedbackCouldNotBeSubmitted = "Feedback could not be submitted";
        public const string FeedbackSuccessfullySubmitted = "Feedback successfully submitted";
        public const string PasswordCouldNotBeReset = "Password could not be reset";
        public const string PasswordResetEmailSuccessfullySent = "Password reset email has been sent successfully";
        public const string NoUserAssociatedWithThisEmailAddress = "No user is currently associated with this email address";
        public const string ProfileCouldNotBeUpdated = "Profile could not be updated";
        public const string UserCouldNotBeDeleted = "User could not be deleted";
        public const string UserSuccessfullyDeleted = "User successfully deleted";
        public const string UserCouldNotBeCreated = "User could not be created";
        public const string UserAssociatedWithEmail = "There is already an user associated with this email address";
        public const string UserAssociatedWithUserName = "There is already an user associated with this user name";
        public const string UserCouldNotBeUpdated = "User could not be updated";
        public const string InvitationEmailCouldNotBeSent = "Invitation email could not be sent";
        public const string InvitationEmailSuccessfullySent = "Invitation email has been sent successfully";
        public const string SubscriptionsSuccessfullySaved = "Subscriptions successfully saved!";
        public const string SubscriptionsCouldNotBeSaved = "Subscriptions could not be saved!";
        public const string ChangesCouldNotBeApproved = "Changes could not be approved!";
        public const string ChangesSuccessfullyApproved = "Changes successfully approved!";
        public const string ChangesCouldNotBeDiscarded = "Changes could not be discarded!";
        public const string ChangesSuccessfullyDiscarded = "Changes successfully discarded!";
        public const string AjaxErrorMessage = "Request could not be processed. Please try again later";
        public const string UserSuccessfullyUpdated = "User successfully updated";
        public const string UserSuccessfullyCreated = "User successfully created";
        public const string AccountSuccessfullyActivated = "Your account is successfully activated.";
        public const string WillBeRedirectedToProfile = "You will be redirected to your profile within";
        public const string RedirectingToProfile = "Redirecting to your profile...";
        public const string ActivationLinkExpired = "Sorry, this activation link has been expired";
        public const string ActivationLinkInvalid = "The activation link you have followed is invalid";
        public const string PasswordResetKeyInvalid = "The password reset link you have followed is invalid";
        public const string PasswordResetLinkExpired = "Sorry, this link has been expired. Please try to reset your password again";

        public const string NotificationSubscriptionsSavingCommandText = "[dbo].[usp_SaveNotificationSubscriptions]";
    }
}