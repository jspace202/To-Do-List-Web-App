<template>
  <div class="header">
    <ul
      v-show="tabs"
      v-if="tabs"
      class="tabs"
    >
      <li
        :class="{ 'gray-text': todoList, 'disable-pointer-events': todoList }"
        @click="$emit('show-todo'), animateList()"
      >
        Todo <strong
          v-show="completedList"
          v-if="incompleteTasksCounter!==0"
        >{{ incompleteTasksCounter }}</strong>
      </li>
      <li
        :class="{ 'gray-text': completedList, 'disable-pointer-events': completedList }"
        @click="$emit('show-completed'), animateList()"
      >
        Completed <strong
          v-show="todoList"
          v-if="completedTasksCounter!==0"
        >{{ completedTasksCounter }}</strong>
      </li>
    </ul>
    <span
      class="material-symbols-outlined toggle-box"
    >
      &#xe164;
    </span>
    <h1 v-if="todoList">
      Todo
    </h1>
    <h1 v-else>
      Completed
    </h1>
  </div>
</template>

<script>
export default {
  name: 'TheHeader',
  props: {
    tasks: {
      type: Array,
      required: true
    },
    tabs: {
      type: Boolean,
      required: true
    },
    todoList: {
      type: Boolean,
      required: true
    },
    completedList: {
      type: Boolean,
      required: true
    }
  },
  data () {
    return {
      // Remove the counter from data()
    }
  },
  computed: {
    completedTasksCounter () {
      return this.tasks.filter(task => task.isCompleted).length
    },
    incompleteTasksCounter () {
      return this.tasks.filter(task => !task.isCompleted).length
    }
  },
  mounted () {
    setTimeout(() => {
      document.querySelector('.header ul').classList.add('show')
    }, 200)
  },
  methods: {
    animateList () {
      const list = document.querySelector('.header ul')
      if (list) {
        list.classList.remove('show')
        setTimeout(() => {
          list.classList.add('show')
        }, 200)
      }
    }
  }
}
</script>

<style scoped>
strong {
  background-color: #e0e5ec00;
  color: #5C5C5C;
  padding: 0px;
  font-weight: normal;
  font-size: 10px;
  padding-right: 6px;
  padding-left: 6px;
  margin-left: 3px;
  border-radius: 2px;
  box-shadow: 2px 2px 6px #C6CDD6, -2px -2px 6px #F1F5FA;
}

.background-grey {
  background-color: gray;
}

.header {
  display: flex;
  align-items: center;
  justify-content: right;
  position:relative
}

.header h1 {
  margin-top: 50px;
  margin-bottom: 50px;
  color: rgb(148, 148, 148);
  margin-right: 20px;
}

.header span {
  display: flex;
  align-items: center;
  justify-content: left;
  width: 100%;
  margin-left: 50px;
  font-size: 40px;
  color: #6798ff;
  cursor: default;
}

.header p{
  cursor: pointer;
  position: absolute;
  width: 40px;
  height: 40px;
  margin: 0px;
}

.header ul li {
  opacity: 0;
  transform: translateX(-20px);
  transition: all 0.3s;
}

.header ul.show li {
  opacity: 1;
  transform: translateX(0);
}

.tabs {
  text-transform: none;
  font-size: 12px;
  padding: 0px;
  padding-left: 5px;
  color: #6798ff;
  position: absolute;
  left: 100px;
}

.tabs li {
  text-decoration: none;
  list-style: none;
  margin-right: 10px;
  cursor: pointer;
}

.tabs li:hover {
  color: gray;
}

.gray-text {
  color: gray;
  font-weight: bold;
}

.disable-pointer-events {
  pointer-events: none;
}

.toggle-box {
  cursor: pointer;
}
</style>
