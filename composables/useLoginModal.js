import { ref, watch } from "vue";

/** @type {import('vue').Ref<boolean>} */
const isLoginModalShow = ref(false);

/**
 * @returns {{
 *   showModal: () => void,
 *   closeModal: () => void,
 *   isLoginModalShow: import('vue').Ref<boolean>
 * }}
 */
export default function useAuthModal() {
  const route = useRoute();
  const router = useRouter();

  function showModal() {
    isLoginModalShow.value = true;
  }

  function closeModal() {
    isLoginModalShow.value = false;

    if (route.query.redirect) {
      const newQuery = { ...route.query };
      delete newQuery.redirect;
      router.replace({ path: route.path, query: newQuery });
    }
  }

  function init() {
    if (route.query.redirect) {
      showModal();
    } else {
      closeModal()
    }
  }

  watch(
    () => route.query.redirect,
    (redirect) => {
      if (redirect) {
        showModal();
      }
    }
  );

  init()

  return {
    showModal,
    closeModal,
    isLoginModalShow,
  };
}
