window.academyFeedback = {
    dismissed: function () {
        try { return sessionStorage.getItem('academyFeedbackDismissed') === '1'; }
        catch (e) { return false; }
    },
    dismiss: function () {
        try { sessionStorage.setItem('academyFeedbackDismissed', '1'); }
        catch (e) { }
    }
};