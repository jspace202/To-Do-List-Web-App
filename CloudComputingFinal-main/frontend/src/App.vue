<template>
  <div>
    <UserLogin
      @user-logged-in="toggleLogin"
      @user-logged-out="toggleLogin"
    />
    <TodoList :is-logged-in="isLoggedIn" />
  </div>
</template>

<script>
import TodoList from './components/TodoList.vue'
import UserLogin from './components/UserLogin.vue'

export default {
  components: {
    TodoList,
    UserLogin
  },
  data () {
    return {
      isLoggedIn: false
    }
  },
  methods: {
    toggleLogin () {
      this.isLoggedIn = !this.isLoggedIn
    }
  }
}
</script>

<style>
@import url('https://fonts.googleapis.com/css?family=Poppins&display=swap');

body {
  background: rgb(255,255,255);
  background: linear-gradient(90deg, rgba(255,255,255,1) 0%, rgba(228,228,242,1) 35%, rgba(249,252,255,1) 67%, rgba(241,252,255,1) 100%);
}

body, html {
  margin: 0;
  padding: 0;
  max-width: 100%;
  overflow-x: hidden;
}

* {
  font-family: Poppins;
}

.material-symbols-outlined {
  user-select: none;
}

strong {
  user-select: none; /* Standard syntax */
}

</style>
