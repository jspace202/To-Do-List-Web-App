<template>
  <div>
    <div v-if="showExtensions">
      <ExtensionViewer
        :logged-in="isLoggedIn"
        @toggle-extension-display="extensionsViewToggle"
        @toggle-canvas-input="$emit('toggle-canvas-input')"
      />
    </div>
    <div
      v-if="type==='buttons'"
      class="footer-wrapper"
    >
      <div
        :class="{ 'disabled': !isLoggedIn }"
        @click="isLoggedIn && extensionsViewToggle()"
      >
        <span
          v-if="isLoggedIn"
          class="material-symbols-outlined blue-circle tip right"
          data-tippy-content="Extensions"
        >&#xf1b7;</span>
        <span
          v-if="!isLoggedIn"
          class="material-symbols-outlined blue-circle tip right"
          data-tippy-content="Login for Extensions"
        >&#xf1b7;</span>
      </div>
      <div @click="$emit('toggle-form-input')">
        <span
          class="material-symbols-outlined blue-circle tip"
          data-tippy-content="Add New Task"
        >&#xe145;</span>
      </div>
    </div>
    <div v-if="type==='form'">
      <div
        v-if="addItem===true"
        class="form-wrapper"
        @click.self="$emit('toggle-form-input')"
      >
        <h4>Add New Task</h4>
        <form @submit.prevent="$emit('submit-form'), resetUserInputs('task')">
          <div
            class="close-form"
            @click="$emit('toggle-form-input')"
          >
            <span class="material-symbols-outlined closer">
              &#xe5cd;
            </span>
          </div>
          <label for="title">Title</label>
          <input
            id="title"
            v-model="title"
            type="text"
            required
          >
          <br>
          <label for="description">Description</label>
          <textarea
            id="description"
            v-model="description"
          />
          <br>
          <label for="dueDate">Due Date</label>
          <input
            id="dueDate"
            v-model="dueDate"
            type="datetime-local"
          >
          <br>
          <div class="submit-section">
            <button type="submit">
              <div
                class="form-submit"
              >
                <span class="material-symbols-outlined blue-circle">&#xe145;</span>
              </div>
            </button>
          </div>
        </form>
      </div>
      <div
        v-if="canvasLogin===true && isLoggedIn===true"
        class="form-wrapper"
        @click.self="$emit('toggle-canvas-input')"
      >
        <h4>Import Canvas Tasks</h4>
        <p>This import should take around 5 minutes to complete</p>
        <form @submit.prevent="$emit('submit-canvas-form-details', { canvasUrl: canvasUrl, accessToken: accessToken }), resetUserInputs('canvas')">
          <div
            class="close-form"
            @click="$emit('toggle-canvas-input')"
          >
            <span class="material-symbols-outlined closer">
              &#xe5cd;
            </span>
          </div>
          <label
            class="canvas-label"
            for="canvas-url"
          >Canvas LMS URL</label>
          <input
            id="canvas-url"
            v-model="canvasUrl"
            type="text"
          >
          <label
            class="canvas-label"
            for="access-token"
          >OAuth Access Token</label>
          <input
            id="access-token"
            v-model="accessToken"
            type="text"
          >
          <div class="submit-section subb">
            <button type="submit">
              <div class="form-submit">
                <span class="material-symbols-outlined blue-circle">&#xe145;</span>
              </div>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import ExtensionViewer from './ExtensionsViewer.vue'

export default {
  name: 'TheFooter',
  components: {
    ExtensionViewer
  },
  props: {
    addItem: {
      type: Boolean,
      required: true
    },
    canvasLogin: {
      type: Boolean,
      required: true
    },
    type: {
      type: String,
      required: true
    },
    isLoggedIn: {
      type: Boolean,
      default: false
    }
  },
  data () {
    return {
      title: '',
      description: '',
      dueDate: '',
      canvasUrl: '',
      accessToken: '',
      showExtensions: false
    }
  },
  watch: {
    title (value) {
      this.$emit('update-title', value)
    },
    description (value) {
      this.$emit('update-description', value)
    },
    dueDate (value) {
      this.$emit('update-dueDate', value)
    }
  },
  methods: {
    resetUserInputs (type) {
      if (type === 'canvas') {
        this.canvasUrl = ''
        this.accessToken = ''
      }
      if (type === 'task') {
        this.title = ''
        this.description = ''
        this.dueDate = ''
      }
    },
    extensionsViewToggle () {
      if (this.isLoggedIn) {
        this.showExtensions = !this.showExtensions
      }
    }
  }
}
</script>

<style scoped>
.footer-wrapper {
  display: flex;
  justify-content: right;
  margin-bottom: 50px;
  margin-top: 50px;
  margin-right: 65px;
}

.subb {
  margin-top: 45px;
}

.footer-wrapper span {
  cursor: pointer;
  background-color: #6798ff;
  border-radius: 50%;
  font-size: 30px;
  padding: 10px;
  color: white;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.1);
}

.footer-wrapper span:hover {
  background-color: #6798ff;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.2);
}

.canvas-icon {
  height: 50px;
  width: 50px;
  cursor: pointer;
  border-radius: 50%;
  font-size: 30px;
  color: white;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38);
  margin-right: 10px;
}

.canvas-icon:hover {
  opacity: .7;
}

.form-wrapper {
  position: absolute;
  width: 100%;
  top: 0px;
  height: 785px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.form-wrapper form {
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

.right {
  margin-right: 5px;
}

.form-wrapper form input, textarea {
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.form-wrapper h4 {
  margin: 0 auto;
  position: absolute;
  z-index: 10000;
  font-size: 25px;
  top: 220px;
  color: #6798ff;
  font-weight: normal;
}

.form-wrapper p {
  margin: 0 auto;
  position: absolute;
  z-index: 10000;
  font-size: 13px;
  top: 260px;
  font-weight: normal;
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

form button {
  border: transparent;
  background-color: white;
}

form button {
  width: 50px;
  height: 50px;
  padding: 0px;
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
  margin-top: 20px;
  display: flex;
  justify-content: center;
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
  background-color: #6798ff;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.2);
}

.form-submit {
  width: 50px;
  height: 50px;
}

.canvas-label {
  margin-bottom: 10px;
  margin-top: 25px;
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
}

.closer:hover{
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

.disabled span {
    color: #ffffff;
    background-color: #e0e0e0;
  }
.disabled span:hover{
  background-color: #e0e0e0;
  box-shadow: 0px 5px 10px 0px rgba(156, 156, 156, 0.38), inset 0px 2px 2px rgba(255, 255, 255, 0.1), inset 0px -2px 2px rgba(0, 0, 0, 0.2);
  cursor:default;
}
</style>
