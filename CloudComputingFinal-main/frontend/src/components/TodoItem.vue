<template>
  <div>
    <div
      v-for="(task, index) in tasks"
      :key="index"
    >
      <div
        v-if="task.isCompleted === false && type==='todo'"
        class="list-item"
      >
        <div class="listed">
          <span
            class="material-symbols-outlined smallCircle"
            :class="{ checked: isChecked[index] }"
            @click="$emit('toggle-completed-task', index)"
            @mouseenter="$emit('set-checked', index)"
            @mouseleave="$emit('remove-checked', index)"
          >
            {{ isChecked[index] ? '&#xe5ca;' : '&#xef4a;' }}
          </span>
          <div
            v-if="fullDisplay[index] !== false"
            class="rowed-lines"
            @click="$emit('show-all-item-contents', index)"
          >
            <strong v-if="fullDisplay[index] !== true"><span>{{ task.title }}</span></strong>
            <div class="full-line">
              <strong><span v-if="fullDisplay[index]">{{ task.title }}</span></strong>
              <span v-if="fullDisplay[index]"><strong>Created: </strong>{{ convertToRegularTime(task.created) }}</span>
              <span v-if="fullDisplay[index]"><strong>Due Date: </strong>{{ convertToRegularTime(task.dueDate) }}</span>
              <span v-if="fullDisplay[index] !==true && task.dueDate != null">Due Date: {{ formatDate(task.dueDate) }}</span>
              <span v-if="fullDisplay[index]">
                <br>
                <strong>Description: </strong>
                <span v-if="convertStringToLink(task.description).isLink">
                  <button
                    class="open-btn"
                    @click="openLink(convertStringToLink(task.description).link)"
                  >
                    View Description
                  </button>
                </span>
                <span v-else>
                  {{ convertStringToLink(task.description).link }}
                </span>
              </span>
            </div>
          </div>
          <div
            v-if="fullDisplay[index] !== true"
            class="icons"
          >
            <div @click="$emit('remove-task', index)">
              <span class="material-symbols-outlined delete">&#xe872;</span>
            </div>
            <div @click="$emit('toggle-favorite', index)">
              <span
                class="material-symbols-outlined"
                :class="['favorite', { active: task.isFavorite }]"
              >&#xe87d;</span>
            </div>
          </div>
        </div>
      </div>
      <div
        v-if="task.isCompleted === true && type==='completed'"
        class="list-item"
      >
        <div class="listed">
          <span
            class="material-symbols-outlined checkmark"
            :class="{ checked: task.isChecked }"
            @click="$emit('toggle-completed-task', index)"
            @mouseenter="$emit('set-checked', index)"
            @mouseleave="$emit('remove-checked', index)"
          >
            {{ isChecked[index] ? '&#xef4a;' : '&#xe5ca;' }}
          </span>
          <div
            v-if="fullDisplay[index] !== false"
            class="rowed-lines"
            @click="$emit('show-all-item-contents', index)"
          >
            <strong v-if="fullDisplay[index] !== true"><span>{{ task.title }}</span></strong>
            <div class="full-line">
              <strong><span v-if="fullDisplay[index]">{{ task.title }}</span></strong>
              <span v-if="fullDisplay[index]"><strong>Created: </strong>{{ convertToRegularTime(task.created) }}</span>
              <span v-if="fullDisplay[index]"><strong>Due Date: </strong>{{ convertToRegularTime(task.dueDate) }}</span>
              <span v-if="fullDisplay[index]">
                <br>
                <strong>Description: </strong>
                <span v-if="convertStringToLink(task.description).isLink">
                  <button
                    class="open-btn"
                    @click="openLink(convertStringToLink(task.description).link)"
                  >
                    View Description
                  </button>
                </span>
                <span v-else>
                  {{ convertStringToLink(task.description).link }}
                </span>
              </span>
            </div>
          </div>
          <div
            v-if="fullDisplay[index] !== true"
            class="icons"
          >
            <div @click="$emit('remove-task', index)">
              <span class="material-symbols-outlined delete">&#xe872;</span>
            </div>
            <div @click="$emit('toggle-favorite', index)">
              <span
                class="material-symbols-outlined"
                :class="['favorite', { active: task.isFavorite }]"
              >&#xe87d;</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'TodoList',
  props: {
    tasks: {
      type: Array,
      required: true
    },
    fullDisplay: {
      type: Array,
      required: true
    },
    isChecked: {
      type: Array,
      required: true
    },
    type: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      isLink: false,
      link: ''
    }
  },
  methods: {
    formatDate (date) {
      const options = { year: 'numeric', month: '2-digit', day: '2-digit' }
      return new Date(date).toLocaleDateString('en-US', options)
    },
    convertToRegularTime (dateString) {
      const date = new Date(dateString)
      date.setUTCHours(date.getUTCHours() - 5)
      const year = date.getFullYear()
      const month = String(date.getMonth() + 1).padStart(2, '0')
      const day = String(date.getDate()).padStart(2, '0')
      const hour = date.getHours() > 12 ? date.getHours() - 12 : date.getHours()
      const minute = String(date.getMinutes()).padStart(2, '0')
      const ampm = date.getHours() >= 12 ? 'pm' : 'am'
      const formattedTime = `${month}/${day}/${year} at ${hour}:${minute} ${ampm}`
      return formattedTime
    },
    convertStringToLink (description) {
      if (description.startsWith('https')) {
        return {
          isLink: true,
          link: description
        }
      } else {
        return {
          isLink: false,
          link: description
        }
      }
    },
    openLink (url) {
      if (typeof window !== 'undefined') {
        window.open(url, '_blank')
      }
    }
  }
}
</script>

<style scoped>
.list-item {
  display: flex;
  padding: 20px;
  border-radius: 20px;
  justify-content: center;
  flex-direction: column;
  align-items: left;
  height: auto;
  border: 0px solid black;
  margin-bottom: 20px;
  margin-left: 35px;
  margin-right: 25px;
  box-shadow: 0px 2px 4px 0px rgba(156, 156, 156, 0.38);
  border: 1px solid rgba(156, 156, 156, 0.38);
}

.listed {
  display: flex;
  flex-direction: row;
  align-items: center;

}

.listed .smallCircle {
  font-size: 15px;
  color: rgb(148, 148, 148);
  cursor: pointer;
}

.listed .smallCircle.checked {
  color: black;
  font-size: 15px;
  font-weight: bold;
  cursor: pointer;
}

.rowed-lines {
  width: 100%;
  margin-left: 20px;
  cursor: pointer;
}

.rowed-lines:hover {
  opacity: .9;
 /* color: #6798ff; */
}

.full-line {
  display: flex;
  flex-direction: column;
  width: 100%;
}

.full-line span {
  width: 100%;
}

.icons {
  display: flex;
  justify-content: right;
  align-items: right;
  width: auto;
  margin-left: 15px;
}

.delete {
  color: black;
  cursor: pointer;
}

.delete:hover {
  color: gray;
}

.favorite {
  color: rgb(0, 0, 0);
  cursor: pointer;
}

.favorite.active {
  color: red;
}

.checkmark {
  font-size: 15px;
  cursor: pointer;
  font-weight: bold;
}

.checkmark.checked{
  color: black;
  font-size: 15px;
  font-weight: bold;
  cursor: pointer;
}

.checkmark:hover {
  font-size: 15px;
  color: rgb(148, 148, 148);
  cursor: pointer;
  font-weight: normal;
}

.open-btn {
  border: none;
  border-radius: 10px;
  padding: 10px;
  font-size: 12px;
  transition: all 0.3s ease;
  text-align: center;
  color: #6798ff;
  background-color: rgba(255, 255, 255, 0);
  margin-left: 5px;
}

.open-btn:hover {
  box-shadow: inset 5px 5px 10px #e0e0e0, inset -5px -5px 10px #ffffff;
  cursor: pointer;
}

</style>
