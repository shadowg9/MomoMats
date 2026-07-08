const userId = "demo-user";


const state = {

    openAiMats: [],

    geminiMats: [],

    cart: {
        items: [],
        total: 0
    },

    orders: []

};



const elements = {

    openAiGrid:
        document.getElementById("openAiGrid"),

    geminiGrid:
        document.getElementById("geminiGrid"),

    cartButton:
        document.getElementById("cartButton"),

    cartPanel:
        document.getElementById("cartPanel"),

    closeCartButton:
        document.getElementById("closeCartButton"),

    cartItems:
        document.getElementById("cartItems"),

    cartTotal:
        document.getElementById("cartTotal"),

    cartCount:
        document.getElementById("cartCount"),

    checkoutButton:
        document.getElementById("checkoutButton"),

    ordersList:
        document.getElementById("ordersList"),

    overlay:
        document.getElementById("overlay"),

    notification:
        document.getElementById("notification")

};



async function apiRequest(url, options = {}) {

    const response = await fetch(url, {

        headers: {
            "Content-Type": "application/json",
            ...(options.headers || {})
        },

        ...options

    });


    if (!response.ok) {

        let message =
            `Request failed with status ${response.status}.`;


        try {

            const error = await response.json();

            if (error.message) {
                message = error.message;
            }

        }
        catch {
            // Use fallback error message.
        }


        throw new Error(message);
    }


    if (response.status === 204) {
        return null;
    }


    return response.json();
}



function formatCurrency(value) {

    return new Intl.NumberFormat(
        "en-US",
        {
            style: "currency",
            currency: "USD"
        }
    ).format(value);

}



function escapeHtml(value) {

    return String(value)

        .replaceAll("&", "&amp;")

        .replaceAll("<", "&lt;")

        .replaceAll(">", "&gt;")

        .replaceAll('"', "&quot;")

        .replaceAll("'", "&#039;");

}



async function loadMats() {

    try {

        const [
            openAiMats,
            geminiMats
        ] = await Promise.all([

            apiRequest("/api/mats/openai"),

            apiRequest("/api/mats/gemini")

        ]);


        state.openAiMats = openAiMats;

        state.geminiMats = geminiMats;


        renderMatCollection(
            elements.openAiGrid,
            state.openAiMats,
            "openai"
        );


        renderMatCollection(
            elements.geminiGrid,
            state.geminiMats,
            "gemini"
        );

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



function renderMatCollection(
    container,
    mats,
    providerClass) {

    container.innerHTML = mats

        .map(mat => {

            const imageContent = mat.imageUrl

                ? `
                    <img
                        src="${escapeHtml(mat.imageUrl)}"
                        alt="${escapeHtml(mat.name)}">
                  `

                : `
                    <div class="generated-placeholder">

                        <div class="mat-shape">
                            <span>
                                ${escapeHtml(mat.name)}
                            </span>
                        </div>

                    </div>
                  `;


            return `

                <article class="mat-card">


                    <div class="mat-image ${providerClass}">

                        ${imageContent}

                        <span class="provider-badge">
                            ${escapeHtml(mat.provider)}
                        </span>

                    </div>


                    <div class="mat-content">

                        <h3>
                            ${escapeHtml(mat.name)}
                        </h3>


                        <p>
                            ${escapeHtml(mat.description)}
                        </p>


                        <div class="mat-footer">

                            <strong>
                                ${formatCurrency(mat.price)}
                            </strong>


                            <button
                                type="button"
                                class="add-button"
                                data-add-mat="${mat.id}">

                                Add to Cart

                            </button>

                        </div>

                    </div>


                </article>

            `;

        })

        .join("");

}



async function loadCart() {

    try {

        state.cart =
            await apiRequest(
                `/api/cart/${userId}`
            );


        renderCart();

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



function renderCart() {

    const items =
        state.cart.items || [];


    const totalQuantity = items.reduce(

        (total, item) =>
            total + item.quantity,

        0

    );


    elements.cartCount.textContent =
        totalQuantity;


    elements.cartTotal.textContent =
        formatCurrency(
            state.cart.total || 0
        );


    if (items.length === 0) {

        elements.cartItems.innerHTML = `

            <div class="empty-state">

                <div class="empty-icon">
                    🛒
                </div>

                <h3>Your cart is empty</h3>

                <p>
                    Choose a mat design from either collection.
                </p>

            </div>

        `;


        elements.checkoutButton.disabled = true;

        return;

    }


    elements.checkoutButton.disabled = false;


    elements.cartItems.innerHTML = items

        .map(item => `

            <article class="cart-item">


                <div>

                    <strong>
                        ${escapeHtml(item.name)}
                    </strong>


                    <span>
                        ${escapeHtml(item.provider)}
                        · Quantity ${item.quantity}
                    </span>

                </div>


                <div class="cart-item-right">

                    <strong>
                        ${formatCurrency(item.lineTotal)}
                    </strong>


                    <button
                        type="button"
                        data-remove-mat="${item.matId}">

                        Remove

                    </button>

                </div>


            </article>

        `)

        .join("");

}



async function addToCart(matId) {

    try {

        state.cart = await apiRequest(

            `/api/cart/${userId}/items`,

            {

                method: "POST",

                body: JSON.stringify({

                    matId: matId,

                    quantity: 1

                })

            }

        );


        renderCart();


        showNotification(
            "Mat added to your cart."
        );

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



async function removeFromCart(matId) {

    try {

        state.cart = await apiRequest(

            `/api/cart/${userId}/items/${matId}`,

            {
                method: "DELETE"
            }

        );


        renderCart();

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



async function checkout() {

    try {

        await apiRequest(

            `/api/orders/${userId}`,

            {
                method: "POST"
            }

        );


        closeCart();


        await Promise.all([

            loadCart(),

            loadOrders()

        ]);


        showNotification(
            "Your mock order was placed successfully."
        );

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



async function loadOrders() {

    try {

        state.orders = await apiRequest(
            `/api/orders/${userId}`
        );


        renderOrders();

    }
    catch (error) {

        showNotification(
            error.message,
            true
        );

    }

}



function renderOrders() {

    if (state.orders.length === 0) {

        elements.ordersList.innerHTML = `

            <div class="empty-orders">

                <p>
                    No orders have been placed yet.
                </p>

            </div>

        `;

        return;

    }


    elements.ordersList.innerHTML = state.orders

        .map(order => `

            <article class="order-card">


                <div>

                    <strong>
                        Order #${order.id}
                    </strong>

                    <span>
                        ${new Date(
            order.createdAt
        ).toLocaleString()}
                    </span>

                </div>


                <p>

                    ${order.items

                .map(item =>
                    `${escapeHtml(item.matName)} × ${item.quantity}`
                )

                .join(", ")}

                </p>


                <div class="order-summary">

                    <span>
                        ${escapeHtml(order.status)}
                    </span>

                    <strong>
                        ${formatCurrency(order.total)}
                    </strong>

                </div>


            </article>

        `)

        .join("");

}



function openCart() {

    elements.cartPanel.classList.add("open");

    elements.overlay.classList.add("visible");

    elements.cartPanel.setAttribute(
        "aria-hidden",
        "false"
    );

}



function closeCart() {

    elements.cartPanel.classList.remove("open");

    elements.overlay.classList.remove("visible");

    elements.cartPanel.setAttribute(
        "aria-hidden",
        "true"
    );

}



function showNotification(
    message,
    isError = false) {

    elements.notification.textContent =
        message;


    elements.notification.classList.toggle(
        "error",
        isError
    );


    elements.notification.classList.add(
        "visible"
    );


    setTimeout(() => {

        elements.notification.classList.remove(
            "visible"
        );

    }, 3000);

}



document.addEventListener(
    "click",
    async event => {


        const addButton =
            event.target.closest(
                "[data-add-mat]"
            );


        if (addButton) {

            await addToCart(

                Number(
                    addButton.dataset.addMat
                )

            );

            return;

        }


        const removeButton =
            event.target.closest(
                "[data-remove-mat]"
            );


        if (removeButton) {

            await removeFromCart(

                Number(
                    removeButton.dataset.removeMat
                )

            );

        }

    }
);



elements.cartButton.addEventListener(
    "click",
    openCart
);


elements.closeCartButton.addEventListener(
    "click",
    closeCart
);


elements.overlay.addEventListener(
    "click",
    closeCart
);


elements.checkoutButton.addEventListener(
    "click",
    checkout
);



async function initializeApplication() {

    await Promise.all([

        loadMats(),

        loadCart(),

        loadOrders()

    ]);

}



initializeApplication();