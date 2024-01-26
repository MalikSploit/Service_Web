window.stripeIntegration = {
    stripe: null,
    card: null,

    initializeStripe: function (publishableKey) {
        this.stripe = Stripe(publishableKey);
        var elements = this.stripe.elements();
        var style = {
            base: {
                color: "#32325d",
                fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
                fontSmoothing: "antialiased",
                fontSize: "16px",
                "::placeholder": {
                    color: "#aab7c4"
                }
            },
            invalid: {
                color: "#fa755a",
                iconColor: "#fa755a"
            }
        };
        this.card = elements.create("card", { style: style });
        this.card.mount("#card-element");
        this.card.addEventListener('change', function (event) {
            var displayError = document.getElementById('card-errors');
            if (event.error) {
                displayError.textContent = event.error.message;
            } else {
                displayError.textContent = '';
            }
        });
    },

    validateCard: function () {
        return new Promise((resolve, reject) => {
            this.stripe.createToken(this.card).then(function (result) {
                if (result.error || !result.token) {
                    // Inform the user if there was an error or no token
                    reject("Card validation failed or no card details provided.");
                } else {
                    // The card is valid
                    resolve(true);
                }
            }).catch(function (error) {
                reject("An error occurred during card validation.");
            });
        });
    }
};
