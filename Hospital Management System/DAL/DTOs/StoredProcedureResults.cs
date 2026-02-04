namespace HospitalManagementSystem.DAL.DTOs
{
    /// <summary>
    /// Result of the RegisterPatient stored procedure.
    /// </summary>
    public sealed class RegisterPatientResult
    {
        /// <summary>
        /// Gets or sets the patient identifier.
        /// </summary>
        public int PatientID { get; set; }

        /// <summary>
        /// Gets or sets the generated patient code.
        /// </summary>
        public string PatientCode { get; set; }
    }

    /// <summary>
    /// Result of the CreateAppointment stored procedure.
    /// </summary>
    public sealed class CreateAppointmentResult
    {
        /// <summary>
        /// Gets or sets the appointment identifier.
        /// </summary>
        public int AppointmentID { get; set; }

        /// <summary>
        /// Gets or sets the generated appointment code.
        /// </summary>
        public string AppointmentCode { get; set; }

        /// <summary>
        /// Gets or sets the error message when creation fails.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Result of the GenerateInvoice stored procedure.
    /// </summary>
    public sealed class GenerateInvoiceResult
    {
        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        public int InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the generated invoice number.
        /// </summary>
        public string InvoiceNumber { get; set; }
    }

    /// <summary>
    /// Result of the ProcessPayment stored procedure.
    /// </summary>
    public sealed class ProcessPaymentResult
    {
        /// <summary>
        /// Gets or sets the payment identifier.
        /// </summary>
        public int PaymentID { get; set; }

        /// <summary>
        /// Gets or sets the payment number.
        /// </summary>
        public string PaymentNumber { get; set; }

        /// <summary>
        /// Gets or sets the updated invoice status.
        /// </summary>
        public string InvoiceStatus { get; set; }
    }
}
