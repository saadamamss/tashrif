export default defineNuxtPlugin(nuxtApp => {
  nuxtApp.vueApp.directive('ripple', {
    mounted(el, binding) {
      el.style.position = 'relative'
      el.style.overflow = 'hidden'
      
      el.addEventListener('click', (e) => {
        // Remove previous ripples
        const ripples = el.getElementsByClassName('ripple-effect')
        while (ripples[0]) {
          ripples[0].parentNode.removeChild(ripples[0])
        }
        
        // Create new ripple
        const circle = document.createElement('span')
        const diameter = Math.max(el.clientWidth, el.clientHeight)
        const radius = diameter / 2
        
        circle.style.width = circle.style.height = `${diameter}px`
        circle.style.left = `${e.clientX - el.getBoundingClientRect().left - radius}px`
        circle.style.top = `${e.clientY - el.getBoundingClientRect().top - radius}px`
        circle.classList.add('ripple-effect')
        
        // Add custom color if provided
        if (binding.value) {
          circle.style.backgroundColor = binding.value
        }
        
        el.appendChild(circle)
        
        // Remove element after animation
        setTimeout(() => {
          circle.remove()
        }, 600)
      })
    }
  })
})