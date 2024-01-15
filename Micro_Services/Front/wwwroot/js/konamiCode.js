let konamiCode = ['ArrowUp', 'ArrowUp', 'ArrowDown', 'ArrowDown', 'ArrowLeft', 'ArrowRight', 'ArrowLeft', 'ArrowRight', 'b', 'a'];
let current = 0;

document.addEventListener('keydown', (e) => {
    if (e.key === konamiCode[current]) {
        current++;

        if (current === konamiCode.length) {
            triggerConfetti();
            current = 0;
        }
    } else {
        current = 0;
    }
});

function triggerConfetti() {
    confetti({
        particleCount: 200,   // Increase the number of particles
        spread: 90,           // Increase the spread angle
        startVelocity: 50,    // Increase the starting velocity
        decay: 0.95,          // To make particles float longer
        origin: { y: 1 }      // Adjust origin
    });
}