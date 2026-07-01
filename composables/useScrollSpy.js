import { ref, onMounted, onBeforeUnmount, nextTick } from "vue";
import { useRouter, useRoute } from "vue-router";

/** @type {import('vue').Ref<string>} */
const currentSection = ref("");

/**
 * @param {number} [offset=110]
 * @returns {{
 *   currentSection: import('vue').Ref<string>,
 *   goToSection: (e: MouseEvent) => void,
 *   scrollToSection: (id: string) => Promise<void>
 * }}
 */
export default function (offset = 110) {
  const router = useRouter();
  const route = useRoute();
  /** @type {IntersectionObserver | null} */
  let observer = null;

  const initObserver = () => {
    if (observer) observer.disconnect();
    observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) currentSection.value = entry.target.id;
      });
    }, { rootMargin: "-20% 0px -70% 0px" });

    document.querySelectorAll("section[id]").forEach(s => observer.observe(s));
  };

  /**
   * @param {string} id
   * @returns {Promise<void>}
   */
  const scrollToSection = async (id) => {
    await nextTick();
    const element = document.getElementById(id);
    if (element) {
      const bodyRect = document.body.getBoundingClientRect().top;
      const elementRect = element.getBoundingClientRect().top;
      const elementPosition = elementRect - bodyRect;
      const offsetPosition = elementPosition - offset;

      window.scrollTo({
        top: offsetPosition,
        behavior: "smooth",
      });
    }
  };

  /**
   * @param {MouseEvent} e
   */
  const goToSection = (e) => {
    e.preventDefault();
    const href = /** @type {HTMLAnchorElement} */ (e.currentTarget).getAttribute("href");
    const id = href.replace(/^\/|#/g, '');

    if (route.path !== '/') {
      router.push("/").then(() => {
        setTimeout(() => scrollToSection(id), 300);
        setTimeout(() => scrollToSection(id), 800);
      });
    } else {
      scrollToSection(id);
    }
  };

  onMounted(() => {
    initObserver();
    const handleScroll = () => {
      const header = document.querySelector("header");
      if (header) header.classList.toggle("scroll-header", window.scrollY > 80);
    };
    window.addEventListener("scroll", handleScroll, { passive: true });
  });

  onBeforeUnmount(() => {
    if (observer) observer.disconnect();
  });

  return { currentSection, goToSection, scrollToSection };
}
