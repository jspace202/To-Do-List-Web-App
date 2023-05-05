<template>
  <div>
    <div class="login-wrapper">
      <button
        v-if="isLoggedIn === false"
        class="login-btn top"
        data-tippy-content="Login / Sign Up"
        @click="toggleLoginModule"
      >
        <span
          class="material-symbols-outlined "
        >
          &#xe7fe;
        </span>
      </button>
      <button
        v-else
        class="login-btn"
        @click="showProfileModule"
      >
        <span
          v-if="isLoggedIn === true"
          class="material-symbols-outlined"
        >
          &#xe7fd;
        </span>
      </button>
      <div
        v-if="profilePage"
        class="profile-page"
      >
        <h3><strong>Welcome back </strong>{{ username }}</h3>
        <form @submit.prevent="changePasswordHandler">
          <div class="cont password-input-wrapper">
            <h5>Change Your Password</h5>
            <label for="old-password">Old Password</label>
            <input
              id="old-password"
              v-model="oldPassword"
              :type="showOldPassword ? 'text' : 'password'"
              required
              pattern="^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{6,}$"
              title="Password must contain at least one uppercase letter, one number, one special character, and be at least 6 characters long"
            >
            <button
              type="button"
              class="password-toggle-btn-two right"
              @click="togglePasswordVisibility('old')"
            >
              <span
                class="material-symbols-outlined password-toggle-icon icon"
              >
                {{ showOldPassword ? '&#xe8f5;' : '&#xe8f4;' }}
              </span>
            </button>
          </div>
          <div class="cont password-input-wrapper">
            <label for="new-password">New Password</label>
            <input
              id="new-password"
              v-model="newPassword"
              :type="showNewPassword ? 'text' : 'password'"
              required
              pattern="^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{6,}$"
              title="Password must contain at least one uppercase letter, one number, one special character, and be at least 6 characters long"
            >
            <button
              type="button"
              class="password-toggle-btn-two right upper"
              @click="togglePasswordVisibility('new')"
            >
              <span
                class="material-symbols-outlined password-toggle-icon icon"
              >
                {{ showNewPassword ? '&#xe8f5;' : '&#xe8f4;' }}
              </span>
            </button>
          </div>
          <div class="button-container last">
            <button
              type="submit"
              class="login-butn-form"
            >
              Change Password
            </button>
          </div>
        </form>
        <div class="button-container">
          <button
            class="signup-btn-form"
            @click="signOut"
          >
            Sign Out
          </button>
        </div>
      </div>
    </div>
    <div
      v-if="loginForm && !signupForm"
      class="log-wrapper"
      @click.self="toggleLoginModule"
    >
      <h4>Login</h4>
      <form @submit.prevent="loginHandler">
        <div
          class="close-form"
          @click="toggleLoginModule"
        >
          <span class="material-symbols-outlined closer">
            &#xe5cd;
          </span>
        </div>
        <label
          class="login-label"
          for="username"
        >Username</label>
        <input
          id="login-url"
          v-model="username"
          type="text"
          required
        >
        <label
          class="login-label"
          for="password"
        >Password</label>
        <div class="password-input-wrapper">
          <input
            id="password"
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            pattern="^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{6,}$"
            title="Password must contain at least one uppercase letter, one number, one special character, and be at least 6 characters long"
          >
          <button
            type="button"
            class="password-toggle-btn"
            @click="togglePasswordVisibility"
          >
            <span
              class="material-symbols-outlined password-toggle-icon"
            >
              {{ showOldPassword ? '&#xe8f5;' : '&#xe8f4;' }}
            </span>
          </button>
        </div>
        <div class="submit-section subb ">
          <button
            type="submit"
            class="login-butn-form"
          >
            Login
          </button>
        </div>
        <div class="s-section subb ">
          <button
            class="signup-btn-form"
            @click.prevent="toggleForm"
          >
            Sign Up
          </button>
        </div>
      </form>
    </div>
    <div
      v-if="loginForm && signupForm"
      class="log-wrapper"
      @click.self="toggleLoginModule"
    >
      <h4>Register</h4>
      <form @submit.prevent="registrationHandler">
        <div
          class="close-form"
          @click="toggleLoginModule"
        >
          <span class="material-symbols-outlined closer">
            &#xe5cd;
          </span>
        </div>
        <label
          class="login-label"
          for="username"
        >Username</label>
        <input
          id="login-url"
          v-model="username"
          type="text"
          required
        >
        <label
          class="login-label"
          for="password"
        >Password</label>
        <div class="password-input-wrapper">
          <input
            id="password"
            v-model="password"
            :type="showPassword ? 'text' : 'password'"
            pattern="^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\S+$).{6,}$"
            title="Password must contain at least one uppercase letter, one number, one special character, and be at least 6 characters long"
          >
          <button
            type="button"
            class="password-toggle-btn"
            @click="togglePasswordVisibility"
          >
            <span
              class="material-symbols-outlined password-toggle-icon"
            >
              {{ showOldPassword ? '&#xe8f5;' : '&#xe8f4;' }}
            </span>
          </button>
        </div>
        <div class="submit-section subb ">
          <button
            type="submit"
            class="login-butn-form"
          >
            Register
          </button>
        </div>
        <div class="s-section subb ">
          <button
            class="signup-btn-form"
            @click.prevent="toggleForm"
          >
            Log In
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import tippy from 'tippy.js'
import 'tippy.js/dist/tippy.css'

// eslint-disable-next-line camelcase
import jwt_decode from 'jwt-decode'

export default {
  data () {
    return {
      username: '',
      password: '',
      oldPassword: '',
      newPassword: '',
      isLoggedIn: false,
      loginForm: false,
      signupForm: false,
      profilePage: false,
      showPassword: false,
      showOldPassword: false,
      showNewPassword: false,
      tokenExpirationTimer: null
    }
  },
  watch: {
    isLoggedIn (newValue) {
      if (newValue) {
        this.startTokenExpirationTimer()
      } else {
        this.stopTokenExpirationTimer()
      }
    }
  },
  mounted () {
    this.loadTippySettings()
    this.checkToken()

    if (this.isLoggedIn) {
      this.$emit('user-logged-in')
    }
  },
  methods: {
    loadTippySettings () {
      tippy('.top', {
        theme: 'custom',
        arrow: true,
        placement: 'bottom'
      })
    },
    toggleLoginModule () {
      this.loginForm = !this.loginForm
    },
    showProfileModule () {
      this.profilePage = !this.profilePage
    },
    togglePasswordVisibility (type) {
      if (type === 'old') {
        this.showOldPassword = !this.showOldPassword
      } else if (type === 'new') {
        this.showNewPassword = !this.showNewPassword
      } else {
        this.showPassword = !this.showPassword
      }
    },
    toggleForm () {
      this.signupForm = !this.signupForm
    },
    async loginHandler () {
      if (this.isLoggedIn) {
        if (this.username !== '' && this.password !== '') {
          try {
            const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/login', {
              username: this.username,
              password: this.password
            }, {
              headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
              }
            })
            console.log(response)
            if (response.data.token) {
              localStorage.setItem('token', response.data.token)
              this.isLoggedIn = true
              this.toggleLoginModule()
              this.$emit('user-logged-in')
            } else {
              // display error message
            }
          } catch (error) {
            console.log(error)
          }
        }
      } else {
        if (this.isLoggedIn) {
          try {
            const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/login', {
              username: this.username,
              password: this.password
            }, {
              headers: {
                Authorization: `Bearer ${localStorage.getItem('token')}`
              }
            })
            console.log(response)
            if (response.data.token) {
              localStorage.setItem('token', response.data.token)
              this.isLoggedIn = true
              this.toggleLoginModule()
              this.$emit('user-logged-in')
            } else {
              // display error message
            }
          } catch (error) {
            console.log(error)
          }
        } else {
          if (this.username !== '' && this.password !== '') {
            try {
              const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/login', {
                username: this.username,
                password: this.password
              })
              console.log(response)
              if (response.data.token) {
                localStorage.setItem('token', response.data.token)
                this.isLoggedIn = true
                this.toggleLoginModule()
                this.$emit('user-logged-in')
              } else {
                // display error message
              }
            } catch (error) {
              console.log(error)
            }
          }
        }
      }
    },
    async registrationHandler () {
      if (this.isLoggedIn) {
        try {
          const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/register', {
            username: this.username,
            password: this.password
          }, {
            headers: {
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          })
          console.log(response.data)
          this.toggleForm()
        } catch (error) {
          console.error(error)
        }
      } else {
        try {
          const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/register', {
            username: this.username,
            password: this.password
          })
          console.log(response.data)
          this.toggleForm()
        } catch (error) {
          console.error(error)
        }
      }
    },
    async changePasswordHandler () {
      try {
        const response = await axios.post(import.meta.env.VITE_API_KEY + '/api/Authenticate/change-password', {
          oldPassword: this.oldPassword,
          newPassword: this.newPassword
        }, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        })
        console.log(response.data)
        this.oldPassword = ''
        this.newPassword = ''
        this.profilePage = false
      } catch (error) {
        console.error(error)
      }
    },
    signOut () {
      localStorage.removeItem('token')
      this.isLoggedIn = false
      this.profilePage = false
      this.username = ''
      this.password = ''
      this.$emit('user-logged-out')
    },
    checkToken () {
      const token = localStorage.getItem('token')
      if (token) {
        const decodedToken = jwt_decode(token)
        this.username = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
        this.isLoggedIn = true
      }
    },
    startTokenExpirationTimer () {
      const token = localStorage.getItem('token')
      if (token) {
        const decodedToken = jwt_decode(token)
        const expirationTime = decodedToken.exp * 1000 // convert expiration time to milliseconds
        const currentTime = new Date().getTime() // get current time in milliseconds
        const timeLeft = expirationTime - currentTime // calculate time left until expiration

        this.tokenExpirationTimer = setTimeout(() => {
          localStorage.removeItem('token')
          this.isLoggedIn = false
        }, timeLeft)
      }
    },
    stopTokenExpirationTimer () {
      clearTimeout(this.tokenExpirationTimer)
    },
    closeProfilePage () {
      this.profilePage = false
    }
  }
}
</script>

<style scoped>
.login-wrapper {
  position: absolute;
  top: 10px;
  right: 10px;
}

.login-btn,
.signup-btn {
  border: none;
  border-radius: 10px;
  padding: 3px;
  width: 40px;
  font-size: 12px;
  transition: all 0.3s ease;
    box-shadow: 5px 5px 10px #e0e0e0, -5px -5px 10px #ffffff;
  background-color: #ffffff;
  text-align: center;
}

.login-butn-form {
  border: none;
  border-radius: 10px;
  padding: 10px;
  width: 120px;
  font-size: 12px;
  transition: all 0.3s ease;
    box-shadow: 5px 5px 10px #e0e0e0, -5px -5px 10px #ffffff;
  background-color: #6798ff;
  text-align: center;
  color: white;
}

.signup-btn-form {
  border: none;
  border-radius: 10px;
  padding: 10px;
  width: 120px;
  font-size: 12px;
  transition: all 0.3s ease;
  text-align: center;
  color: #6798ff;
  background-color: rgba(255, 255, 255, 0);
}

.signup-btn-form:hover{
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

.login-butn-form:hover {
  box-shadow: inset 1px 1px 1px #ffffff21, inset -1px -1px 1px #ffffff;
  cursor: pointer;
}

span {
  font-size: 24px;
}

.login-btn:hover,
.signup-btn:hover {
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

.log-wrapper {
  position: absolute;
  width: 100%;
  top: 0px;
  height: 785px;
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 9999;
}

.login-label {
  margin-bottom: 10px;
  margin-top: 25px;
}

.log-wrapper form {
  position: relative;
  z-index:1000;
  margin: 0 auto;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
  border-radius: 10px;
  background-color: white;
  padding: 20px;
  padding-left: 100px;
  padding-right: 100px;
  padding-top: 50px;
  width: 400px;
  height: 350px;
  display: flex;
  justify-content: center;
  align-items: left;
  flex-direction: column;
  font-weight: bold;
}

.log-wrapper form input, textarea {
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.right {
  margin-right: 5px;
}

.log-wrapper form input, textarea {
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.log-wrapper h4 {
  margin: 0 auto;
  position: absolute;
  z-index: 10000;
  font-size: 25px;
  top: 220px;
  color: #6798ff;
  font-weight: normal;
}

.closer {
  border: none;
  border-radius: 10px;
  padding: 3px;
  width: 40px;
  font-size: 12px;
  transition: all 0.3s ease;
    box-shadow: 5px 5px 10px #e0e0e0, -5px -5px 10px #ffffff;
  background-color: #ffffff;
  cursor: pointer;
    background-color: #6797ff00;
    font-size: 30px;
    color: rgb(0, 0, 0);
    text-align: center;

}

.closer:hover{
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

form label {
  margin-bottom: 5px;
}

textarea:focus {
  outline: 2px solid #6798ff;
}

input {
  height: 30px;
  border-radius: 10px;
  font-size: 12px;
}

textarea {
  height: 30px;
  border-radius: 10px;
  font-size: 12px;
}

input:focus {
  outline: 2px solid #6798ff;
}

#password {
  width: 100%;
}

.close-form {
  display: flex;
  justify-content: right;
  height: auto;
  position: absolute;
  top: 10px;
  right: 10px;
  text-align: center;
}

.close-form span{
  cursor: pointer;
  background-color: #6797ff00;
  font-size: 30px;
  color: rgb(0, 0, 0);
}

.submit-section {
  margin-top: 45px;
  display: flex;
  justify-content: center;
}

.s-section {
  margin-top: 5px;
  display: flex;
  justify-content: center;
}

h5 {
  font-size: 15px;
  text-align: center;
  color: rgb(148, 148, 148);
}

.form-submit span {
  cursor: pointer;
  background-color: #6798ff;
  border-radius: 50%;
  font-size: 30px;
  padding: 10px;
  color: white;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.1);
}

.form-submit span:hover {
  background-color: #83aaff;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.2);
}

.form-submit {
  width: 50px;
  height: 50px;
}

.login-label {
  margin-bottom: 10px;
  margin-top: 25px;
}

.password-input-wrapper {
  position: relative;
}

.password-toggle-btn {
  position: absolute;
  top: 58%;
  right: 0px;
  transform: translateY(-50%);
  background: transparent;
  border: none;
  cursor: pointer;
}

.password-toggle-btn-two {
  position: absolute;
  top: 88%;
  right: 18px;
  transform: translateY(-50%);
  background: transparent;
  border: none;
  cursor: pointer;
  justify-content: right;
  max-width: 10px;
}

.icon {
  position: relative;
}

.upper {
  top: 76%;
}

.password-toggle-icon {
  font-size: 18px;
  color: #aaa;
}

.profile-page {
  position: absolute;
  right: 0px;
  padding: 20px;
  pointer-events: auto;
  z-index: 9999;

  border-radius: 10px;
  background-color: white;
  box-shadow: 25px 40px 28px 0px rgba(156, 156, 156, 0.38);
  margin-top: 15px;
  min-height: 400px;
}

.profile-page h3 {
  color: #83aaff;
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 20px;
  text-align: center;
}

.profile-page form {
  display: flex;
  flex-direction: column;
}

.profile-page form .cont {
  display: flex;
  flex-direction: column;
  margin-bottom: 20px;
}

.profile-page form label {
  color: black;
  font-size: 13px;
  font-weight: bold;
  margin-bottom: 5px;
}

.profile-page form input {
  border: none;
  border-radius: 10px;
  padding: 3px;
  width: 300px;
  font-size: 12px;
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  background-color: #ffffff;
  text-align: left;
}

.profile-page form input:focus {
  outline: 2px solid #6798ff;
}

.profile-page form button[type="submit"],
.profile-page button {
  width: 140px;
}

.button-container {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 5px;
}

.last {
  margin-top: 20px;
}

strong {
  color: black;
  font-weight: normal;
  font-size: 18px;
  user-select:auto;
}
</style>
