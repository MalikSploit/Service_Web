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
        particleCount: 200,  
        spread: 90,          
        startVelocity: 50,   
        decay: 0.95,       
        origin: { y: 1 }      
    });
}