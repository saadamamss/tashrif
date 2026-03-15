import { ref, onMounted, onBeforeUnmount, nextTick } from "vue";
import { useRouter, useRoute } from "vue-router";

const currentSection = ref("");

export default function (offset = 110) {
  const router = useRouter();
  const route = useRoute();
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

  const scrollToSection = async (id) => {
    await nextTick(); // ننتظر الـ DOM يجهز
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

  const goToSection = (e) => {
    e.preventDefault();
    const href = e.currentTarget.getAttribute("href");
    const id = href.replace(/^\/|#/g, '');

    if (route.path !== '/') {
      router.push("/").then(() => {
        // ننتظر وقت كافي للتحميل ثم نقوم بالسكرول
        setTimeout(() => scrollToSection(id), 300);
        // تأكيد إضافي بعد اكتمال تحميل الصور المحتمل
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