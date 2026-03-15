import { ref, watch } from "vue";

const isLoginModalShow = ref(false);
export default function useAuthModal() {
  const route = useRoute();
  const router = useRouter();

  function showModal() {
    isLoginModalShow.value = true;
  }

  function closeModal() {
    isLoginModalShow.value = false;

    // Clean up the redirect query without causing navigation loop
    if (route.query.redirect) {
      const newQuery = { ...route.query };
      delete newQuery.redirect;
      router.replace({ path: route.path, query: newQuery });
    }
  }

  function init() {
    // Immediate check on composable initialization
    if (route.query.redirect) {
      showModal();
    }else{
      closeModal()
    }
  }

  // Watch for route changes
  watch(
    () => route.query.redirect,
    (redirect) => {
      if (redirect) {
        showModal();
      }
    }
  );


  // 
  init()
  // 
  return {
    showModal,
    closeModal,
    isLoginModalShow,
  };
}
